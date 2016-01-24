using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Client.Services;

namespace Mise.Inventory.Services.Implementation
{
	public class PurchaseOrderService : IPurchaseOrderService
	{
		readonly ILoginService _loginService;
		readonly IInventoryService _inventoryService;
		readonly IPARService _parService;
		readonly IVendorService _vendorService;
		readonly IPurchaseOrderRepository _poRepository;
		readonly IInventoryAppEventFactory _eventFactory;
		readonly IReceivingOrderRepository _roRepository;
		IPurchaseOrder _currentPO;
		public PurchaseOrderService(ILoginService loginService, IInventoryService inventoryService, 
			IPARService parService, IVendorService vendorService, IPurchaseOrderRepository poRepos, IReceivingOrderRepository roRepos, 
			IInventoryAppEventFactory eventFactory){
			_loginService = loginService;
			_inventoryService = inventoryService;
			_parService = parService;
			_poRepository = poRepos;
			_roRepository = roRepos;
			_vendorService = vendorService;
			_eventFactory = eventFactory;
		}

		#region IPurchaseOrderService implementation
		public async Task<IPurchaseOrder> CreatePurchaseOrder ()
		{
			var emp = await _loginService.GetCurrentEmployee ();
			if (emp == null) {
				throw new InvalidOperationException ("No employee set!");
			}

			var rest = await _loginService.GetCurrentRestaurant ();
		    if (rest == null)
		    {
		        throw new InvalidOperationException("No restaurant set!");
		    }

			var par = await _parService.GetCurrentPAR ();
			if (par == null) {
				throw new InvalidOperationException ("No PAR set!");
			}

			var inventory = await _inventoryService.GetLastCompletedInventory ();
			if (inventory == null || inventory.DateCompleted.HasValue == false) {
				throw new InvalidOperationException ("You haven't finished an inventory yet, can't create a purchase order");
			}

			//get all the ROs after the last inventory
			var ros = _roRepository.GetAll().Where(ro => ro.DateReceived > inventory.DateCompleted.Value);
			var roItems = ros.SelectMany (ro => ro.GetBeverageLineItems ()).ToList ();

			//create the PO
			var createEvent = _eventFactory.CreatePurchaseOrderCreatedEvent (emp);
			_currentPO = _poRepository.ApplyEvent (createEvent);

			//pair up the line items from the Inventory and the PAR!
			ICollection<IInventoryBeverageLineItem> invLIs;
			invLIs = inventory.GetBeverageLineItems ().ToList ();

			var poEvents = new List<IPurchaseOrderEvent> ();
			foreach (var parLI in par.GetBeverageLineItems()) {
				var invLIsMatchingParLI = GetInventoryItemForPARItem (parLI, invLIs).ToList();

				var roMatches = GetROItemsForParItem (parLI, roItems).ToList();
				decimal diff = parLI.Quantity;
			    if (invLIsMatchingParLI.Any())
			    {
			        var totalInvQuant = invLIsMatchingParLI.Sum(li => li.Quantity);
			       //TODO - do we want to do this by amounts to be better?
			       diff = (parLI.Quantity - totalInvQuant);
			    }

				if (roMatches.Any ()) {
					var receivedItemsSince = roMatches.Sum (li => li.Quantity);
					diff = diff - receivedItemsSince;
				}

			    if (diff > 0) {
					//see if we have a vendor
					var vendor = await _vendorService.GetBestVendorForItem(parLI, diff, rest);

					var numBottles = (int)Math.Ceiling (diff);
					//make an event
					var realLI = parLI as ParBeverageLineItem;
					var addLIEvent = _eventFactory.CreatePOLineItemAddedFromInventoryCalcEvent (emp, _currentPO, realLI,
						numBottles, null, vendor);

					poEvents.Add (addLIEvent);
				}
			}

			if (poEvents.Any ()) {
				_currentPO = _poRepository.ApplyEvents (poEvents);
			}

			return _currentPO;
		}

		private static IEnumerable<IInventoryBeverageLineItem> GetInventoryItemForPARItem(IBaseBeverageLineItem parLI, IEnumerable<IInventoryBeverageLineItem> invLI)
		{
		    return invLI.Where(inv => BeverageLineItemEquator.AreSameBeverageLineItem(parLI, inv));
		}

		private static IEnumerable<IReceivingOrderLineItem> GetROItemsForParItem(IBaseBeverageLineItem parLI, IEnumerable<IReceivingOrderLineItem> roLIs){
			return roLIs.Where(rLI => BeverageLineItemEquator.AreSameBeverageLineItem(parLI, rLI));
		}

	    public async Task SubmitPO (IPurchaseOrder order)
		{
			var emp = await _loginService.GetCurrentEmployee ();

			//TODO send this to server to be emailed
		
			//mark that the PO is awaiting shipments for all vendors we have
			var vendorsFound = _currentPO.GetPurchaseOrderPerVendors ()
				.Where (pvv => pvv.VendorID.HasValue)
				.Select (pvv => pvv.VendorID.Value);

			var events = vendorsFound.Select (vendorID => 
				_eventFactory.CreatePurchaseOrderSentToVendorEvent (emp, _currentPO, vendorID));

			_currentPO = _poRepository.ApplyEvents (events);
			await _poRepository.Commit (order.Id);
		}

		#endregion

		public Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersWaitingForVendor (IVendor vendor)
		{
			var res = (
                from po in _poRepository.GetAll() 
                let perVend = po.GetPurchaseOrderPerVendors().Any(pov => pov.VendorID.HasValue && pov.VendorID.Value == vendor.Id && pov.IsExpectingShipments()) 
                where perVend 
                select po
            ).ToList();

		    return Task.FromResult (res.AsEnumerable ());
		}

        public async Task MarkItemsAsReceivedForPurchaseOrder(IReceivingOrder ro, PurchaseOrderStatus status)
        {
            var emp = await _loginService.GetCurrentEmployee();
			//get our POS
            if (ro.PurchaseOrderID.HasValue)
            {
                var po = _poRepository.GetByID(ro.PurchaseOrderID.Value);

                var events = _eventFactory.CreatePurchaseOrderRecievedFromVendorEvent(emp, po, ro, status);

                _poRepository.ApplyEvent(events);

                await _poRepository.Commit(po.Id);
            }
        }

        public Task<bool> IsPurchaseOrderTotallyFufilledByReceivingOrder(IReceivingOrder ro)
        {
		    if (ro.PurchaseOrderID.HasValue == false)
		    {
		        //no POS, nothing to compare!
		        return Task.FromResult(true);
		    }

		    var po= _poRepository.GetByID(ro.PurchaseOrderID.Value);

            var poPerVendor =
                po.GetPurchaseOrderPerVendors()
                    .FirstOrDefault(pov => pov.VendorID.HasValue && pov.VendorID.Value == ro.VendorID);
            if (poPerVendor == null)
            {
                return Task.FromResult(false);
            }
            var poLineItems = poPerVendor.GetLineItems();

            //check each po line item has a line in the RO, and the quantity is matching
			//check if we got all our items
		    var roLineItems = ro.GetBeverageLineItems().ToList();
		    foreach (var poLI in poLineItems)
		    {
		        var matchingReceivedLine =
		            roLineItems.FirstOrDefault(roLI => BeverageLineItemEquator.AreSameBeverageLineItem(poLI, roLI));
		        if (matchingReceivedLine == null)
		        {
		            return Task.FromResult(false);
		        }

		        if (matchingReceivedLine.Quantity != poLI.Quantity)
		        {
		            return Task.FromResult(false);
		        }
		    }

		    return Task.FromResult(true);
		}
	}
}

