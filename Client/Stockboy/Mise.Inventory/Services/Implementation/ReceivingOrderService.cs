using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.Entities.Inventory;
using Mise.Core.Repositories;
using System.Threading.Tasks;

using Mise.Core.Common.Events;
using Mise.Core.Entities.People;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Common.Entities.Inventory;
using System.Drawing;


namespace Mise.Inventory.Services.Implementation
{
	public class ReceivingOrderService : IReceivingOrderService
	{
		private IReceivingOrder _currentRO;

		readonly IReceivingOrderRepository _receivingOrderRepository;
		readonly IInventoryService _inventoryService;
		readonly IInventoryAppEventFactory _eventFactory;
		readonly ILoginService _loginService;
		readonly IPurchaseOrderService _poService;
		readonly IVendorService _vendorService;
		readonly IInsightsService _insights;
		readonly ILogger _logger;
		public ReceivingOrderService(ILogger logger, IReceivingOrderRepository receivingOrderRepository, 
			IInventoryService inventoryService, IInventoryAppEventFactory eventFactory, 
			ILoginService loginService, IVendorService vendorService, IPurchaseOrderService poService,
		IInsightsService insights)
		{
			_receivingOrderRepository = receivingOrderRepository;
			_inventoryService = inventoryService;
			_eventFactory = eventFactory;
			_loginService = loginService;
			_logger = logger;
			_vendorService = vendorService;
			_poService = poService;
			_insights = insights;
			_currentRO = null;
		}

		public async Task<IReceivingOrder> StartReceivingOrder(IPurchaseOrder po){
			var emp = await GetCurrentEmployee();
			if (emp == null) {
				throw new InvalidOperationException ("Current employee not set");
			}

			var vendor = await _vendorService.GetSelectedVendor ();
			if(vendor == null)
			{
				throw new InvalidOperationException ("Current vendor not set");
			}

			if(_currentRO != null){
				//cancel the current RO
			}
			if(po.GetPurchaseOrderPerVendors ().Select(pov => pov.VendorID).Contains (vendor.ID) == false){
				throw new InvalidOperationException("Selected vendor does not match PurchaseOrder vendor");
			}
				
			var createdEv = _eventFactory.CreateReceivingOrderCreatedEvent (emp, vendor);

			_currentRO = _receivingOrderRepository.ApplyEvent (createdEv);

			var associated = _eventFactory.CreateReceivingOrderAssociatedWithPOEvent (emp, _currentRO, po);
			_currentRO = _receivingOrderRepository.ApplyEvent (associated);

			var vendorSections = po.GetPurchaseOrderPerVendors ()
				.Where (pov => pov.VendorID.HasValue && pov.VendorID.Value == vendor.ID);
			foreach(var vs in vendorSections){
				foreach(var li in vs.GetLineItems ()){
					var liEvent = _eventFactory.CreateReceivingOrderLineItemAddedEvent (emp, li, li.Quantity, 
						_currentRO);

					_currentRO = _receivingOrderRepository.ApplyEvent (liEvent);
				}
			}

			return _currentRO;
		}

		public async Task<IReceivingOrder> StartReceivingOrderForSelectedVendor()
		{
			if(_currentRO != null){
				//TODO cancel the RO
			}
			var emp = await GetCurrentEmployee();
			if (emp == null) {
				throw new InvalidOperationException ("Current employee not set");
			}

			var vendor = await _vendorService.GetSelectedVendor ();
			if(vendor == null)
			{
				throw new InvalidOperationException ("Current vendor not set");
			}
				
				
			var createdEv = _eventFactory.CreateReceivingOrderCreatedEvent (emp, vendor);

			_currentRO = _receivingOrderRepository.ApplyEvent (createdEv);

			return _currentRO;
		}

		public async Task<IReceivingOrder> GetCurrentReceivingOrder ()
		{
			var vendor = await _vendorService.GetSelectedVendor ();
			if(vendor == null)
			{
				throw new InvalidOperationException ("Current vendor not set");
			}
			/*
			if(_currentRO != null && _currentRO.VendorID != vendor.ID){
				//TODO - send a cancel event for the RO already here
				_receivingOrderRepository.CancelTransaction (_currentRO.ID);
				_currentRO = null;
			}


			if (_currentRO == null)
			{
				_currentRO = _receivingOrderRepository.GetAll()
					.FirstOrDefault(ro => ro.VendorID == vendor.ID && ro.Status == ReceivingOrderStatus.Created);
			}*/

			return _currentRO;
		}

		private IReceivingOrder _roToComplete;
		public async Task<bool> CompleteReceivingOrderForSelectedVendor(string notes, string invoiceID)
		{
			try{
				var emp = await GetCurrentEmployee();

				_roToComplete = _currentRO;


				//get all the line items, tell the inventory and vendors about them
				var lineItems = _roToComplete.GetBeverageLineItems().ToList();

				var validItems = lineItems.Where (li => li.ZeroedOut == false);
				if (validItems.Any ()) {
					await _vendorService.AddLineItemsToVendorIfDontExist (_roToComplete.VendorID, validItems).ConfigureAwait (false);
				}

			    var res = true;

				//mark the RO as closed
				var compEvent = _eventFactory.CreateReceivingOrderCompletedEvent(emp, _roToComplete, notes, invoiceID);
				var updatedOrder = _receivingOrderRepository.ApplyEvent (compEvent);
			    if (updatedOrder.Status == ReceivingOrderStatus.Completed)
			    {
	                //do we have a PO associated with this RO?  If so, we need to update it as well
	                if(_currentRO.PurchaseOrderID.HasValue)
	                {
	                    res = await _poService.IsPurchaseOrderTotallyFufilledByReceivingOrder(_currentRO).ConfigureAwait (false);
	                }

			        _currentRO = null;
			    }
			    else
			    {
			        throw new Exception("Unable to complete receiving order");
			    }

					
				return res;
			} catch(Exception e){
				_logger.HandleException (e);
				return false;
			}
		}

		public async Task CommitCompletedOrder(PurchaseOrderStatus status){
			if(_roToComplete.PurchaseOrderID.HasValue){
				await _poService.MarkItemsAsReceivedForPurchaseOrder (_roToComplete, status).ConfigureAwait (false);
			}
			await _receivingOrderRepository.Commit (_roToComplete.ID).ConfigureAwait (false);
			_insights.Track ("Receiving Order Committed", "RO ID", _roToComplete.ID.ToString ());
		    _roToComplete = null;
		}

		public async Task<IReceivingOrderLineItem> AddLineItemToCurrentReceivingOrder (IBaseBeverageLineItem sourceItem, int quantity)
		{
			var emp = await GetCurrentEmployee();
			if(_currentRO == null){
				throw new InvalidOperationException ("No RO found for vendor");
			}

			var alEv = _eventFactory.CreateReceivingOrderLineItemAddedEvent(emp, sourceItem, quantity, _currentRO);

			var ro =_receivingOrderRepository.ApplyEvent (alEv);
			return ro.GetBeverageLineItems ().FirstOrDefault (li => 
				BeverageLineItemEquator.AreSameBeverageLineItem (sourceItem, li));
		}

		public async Task<IReceivingOrderLineItem> AddLineItemToCurrentReceivingOrder (string name, ICategory category,
			string upc, int quantity, int caseSize, LiquidContainer container)
		{
			var emp = await GetCurrentEmployee ();
			var categories = new List<ItemCategory>{ category as ItemCategory };
			var alEv = _eventFactory.CreateReceivingOrderLineItemAddedEvent (emp, name, upc, categories, caseSize, container, 
				quantity, _currentRO);

			var ro = _receivingOrderRepository.ApplyEvent (alEv);

			return ro.GetBeverageLineItems ().FirstOrDefault (li => 
				BeverageLineItemEquator.IsItem (li, name, upc));
		}

		public async Task UpdateQuantityOfLineItem (IReceivingOrderLineItem li, int newQuantity, Money pricePerBottle)
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var order = _currentRO;

			var updateEv = _eventFactory.CreateROLineItemUpdateQuantityEvent (emp, order, li.ID, newQuantity, pricePerBottle);
			_currentRO = _receivingOrderRepository.ApplyEvent (updateEv);
		}

		public async Task ZeroOutLineItem (IReceivingOrderLineItem li)
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var ev = _eventFactory.CreateReceivingOrderLineItemZeroedOutEvent (emp, _currentRO, li.ID);
			_currentRO = _receivingOrderRepository.ApplyEvent (ev);
		}

		async Task<IEmployee> GetCurrentEmployee(){
			var emp = await _loginService.GetCurrentEmployee ();
			if (emp == null) {
				throw new InvalidOperationException ("Current employee not set");
			}
			return emp;
		}
			
	}
}

