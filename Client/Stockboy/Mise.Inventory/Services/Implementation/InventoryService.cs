using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.Entities.Inventory.Events;


namespace Mise.Inventory.Services.Implementation
{
	public class InventoryService : IInventoryService
	{
		readonly IInventoryRepository _inventoryRepository;
		readonly ILoginService _loginService;
		readonly IInventoryAppEventFactory _eventFactory;
		readonly ILogger _logger;
	    readonly IInsightsService _insights;

		IInventoryBeverageLineItem SelectedLineItem{get;set;}
		IInventory SelectedInventory{get;set;}
		IInventory LastCompletedInventory{ get; set; }
	    private IInventorySection _selectedInventorySection;
		public InventoryService (ILogger logger, ILoginService loginService, 
			IInventoryRepository inventoryRespository, IInventoryAppEventFactory eventFactory, IInsightsService insightsService)
		{
			_logger = logger;
			_inventoryRepository = inventoryRespository;
			_loginService = loginService;
			_eventFactory = eventFactory;
		    _insights = insightsService;
		}

		#region IInventoryService implementation

		public Task LoadLatest(){
			var restID = _eventFactory.RestaurantID;
			if (restID.HasValue) {
				SelectedInventory = _inventoryRepository.GetAll ()
					.Where (i => i.RestaurantID == restID.Value)
					.Where (i => i.DateCompleted.HasValue == false)
					.OrderByDescending (i => i.LastUpdatedDate)
					.FirstOrDefault ();

				LastCompletedInventory = _inventoryRepository.GetAll ()
					.Where(i => i.RestaurantID == restID.Value)
					.Where (i => i.DateCompleted.HasValue)
					.OrderByDescending (i => i.DateCompleted.Value)
					.FirstOrDefault ();
			}

			return Task.FromResult (true);
		}

		public Task<IInventory> GetCurrentInventory ()
		{
			return Task.FromResult (SelectedInventory);
		}

		public async Task<IEnumerable<IInventoryBeverageLineItem>> GetLineItemsForCurrentSection ()
		{
			var rest = await _loginService.GetCurrentRestaurant ();
			var inv = await _inventoryRepository.GetCurrentInventory (rest.ID);

			if (inv == null) {
				throw new InvalidOperationException ("No current inventory to get line items for!");
			}

		    var invSection = _selectedInventorySection;
			if (invSection != null) {
				return invSection.GetInventoryBeverageLineItemsInSection ().OrderBy (li => li.InventoryPosition);
			}

		    throw new InvalidOperationException ("No current inventory section!");
		}

		public async Task StartNewInventory ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var rest = await _loginService.GetCurrentRestaurant ();

			var createEvent = _eventFactory.CreateInventoryCreatedEvent (emp);

			SelectedInventory = _inventoryRepository.ApplyEvent (createEvent);

            //create the sections
		    var sectionEvents =
		        rest.GetInventorySections()
		            .Select(rs => _eventFactory.CreateInventoryNewSectionAddedEvent(emp, SelectedInventory, rs));
		    SelectedInventory = _inventoryRepository.ApplyEvents(sectionEvents);

			var currEv = _eventFactory.CreateInventoryMadeCurrentEvent (emp, SelectedInventory);

			SelectedInventory = _inventoryRepository.ApplyEvent (currEv);

			//do we have a previous inventory?  if so, take the LIs from there
			var previous = _inventoryRepository.GetAll ()
				.Where(i => i.RestaurantID == rest.ID)
                .Where(i => i.DateCompleted.HasValue)
				.OrderByDescending (i => i.CreatedDate)
				.FirstOrDefault ();
		    if (previous != null)
		    {
		        try
		        {
		            var events = new List<IInventoryEvent>();

		            //we need to have the restaurant sections that are in both the old, and the new
		            var oldSections = previous.GetSections();

		            foreach (var oldInventorySection in oldSections)
		            {
		                var newSection =
		                    SelectedInventory.GetSections()
		                        .FirstOrDefault(newSec =>
		                                newSec.RestaurantInventorySectionID == oldInventorySection.RestaurantInventorySectionID);
		                if (newSection != null)
		                {
		                    var lisInSection = oldInventorySection.GetInventoryBeverageLineItemsInSection()
		                        .Where(li => li.Quantity > 0).ToList();
		                    if (lisInSection.Any())
		                    {
		                        foreach (var li in lisInSection)
		                        {
		                            var ev = _eventFactory.CreateInventoryLineItemAddedEvent(emp,
		                                li, 0, li.PricePaid, li.VendorBoughtFrom,
		                                newSection, li.InventoryPosition, SelectedInventory);
		                            events.Add(ev);
		                        }
		                    }
		                }
		            }

		            if (events.Any())
		            {
		                SelectedInventory = _inventoryRepository.ApplyEvents(events);
		            }
		        }
		        catch (Exception e)
		        {
		            _logger.HandleException(e);
		            throw;
		        }
		    }
			try{
				await _inventoryRepository.Commit (SelectedInventory.ID);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (string name, ICategory category,
			string upc, int quantity, int caseSize, LiquidContainer container, Money pricePaid)
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

            //get our max inventory position
		    var inventoryPosition = 0;
		    var invSection = _selectedInventorySection;
		    if (invSection != null)
		    {
		        inventoryPosition = invSection.GetNextItemPosition();
		    }

			var categories = new []{ category as ItemCategory };
			var addEv = _eventFactory.CreateInventoryLineItemAddedEvent (emp, name, upc, categories, caseSize, container, quantity, pricePaid, null, _selectedInventorySection, 
                inventoryPosition, SelectedInventory);
		
			SelectedInventory = _inventoryRepository.ApplyEvent (addEv);

			return SelectedInventory.GetBeverageLineItems ().FirstOrDefault (li => 
				BeverageLineItemEquator.IsItem (li, name, upc)
			);
		}

		public async Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (IBaseBeverageLineItem source, int quantity, Money pricePaid)
		{
			if(SelectedInventory == null){
				throw new InvalidOperationException ("Cannot add line item without selected inventory!");
			}
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var addEv = _eventFactory.CreateInventoryLineItemAddedEvent (emp, source, quantity, pricePaid, null, _selectedInventorySection, SelectedInventory.GetBeverageLineItems().Count() + 1, SelectedInventory);
		
			SelectedInventory = _inventoryRepository.ApplyEvent (addEv);

			return SelectedInventory.GetBeverageLineItems ()
				.FirstOrDefault (li => BeverageLineItemEquator.AreSameBeverageLineItem (source, li));
		}

		/*
		public async Task AddLineItemsFromReceivedOrderToRunningInventory(IEnumerable<IReceivingOrderLineItem> lis, Guid vendorID){
			//update our current inventory with this RO

			var emp = await _loginService.GetCurrentEmployee ();
			if(CurrentRunningInventory == null){
				//TODO - what do we do here?  make a new one?
				var makeNew = _eventFactory.CreateInventoryCreatedEvent (emp);
				CurrentRunningInventory = _inventoryRepository.ApplyEvent (makeNew);

				var currentEv = _eventFactory.CreateInventoryMadeCurrentEvent (emp, CurrentRunningInventory);
				CurrentRunningInventory = _inventoryRepository.ApplyEvent (currentEv);
			}
				
			//also see if our vendor doesn't already have this item
			var invEvents = new List<IInventoryEvent>();
			foreach(var li in lis){
				var invEvent = _eventFactory.CreateInventoryLineItemAddedEvent (emp, li, li.Quantity, li.LineItemPrice, 
					vendorID, null, CurrentRunningInventory);
				invEvents.Add (invEvent);
			}

			if (invEvents.Any ()) {
				_inventoryRepository.ApplyEvents (invEvents);
				await _inventoryRepository.Commit (CurrentRunningInventory.ID);
			}

		}*/

	    public Task SetCurrentInventorySection(IInventorySection section)
	    {
	        _selectedInventorySection = section;
	        return Task.FromResult(true);
	    }

	    public async Task MarkSectionAsComplete ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var compEv = _eventFactory.CreateInventorySectionCompletedEvent (emp, SelectedInventory, _selectedInventorySection);

			SelectedInventory = _inventoryRepository.ApplyEvent (compEv);
		}

		public async Task MarkInventoryAsComplete ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var compEv = _eventFactory.CreateInventoryCompletedEvent (emp, SelectedInventory);

			_inventoryRepository.ApplyEvent (compEv);

			try{
				await _inventoryRepository.Commit (SelectedInventory.ID).ConfigureAwait (false);
			} catch(Exception e){
				_logger.HandleException (e);
			}

			//TODO our current should be copied over from here
			_insights.Track("Completed Inventory", "Inventory ID", SelectedInventory.ID.ToString ());
			LastCompletedInventory = SelectedInventory;
			SelectedInventory = null;
		}

		public Task MarkLineItemForMeasurement (IInventoryBeverageLineItem li)
		{
			SelectedLineItem = li;
			return Task.FromResult (false);
		}

		public Task<IInventoryBeverageLineItem> GetLineItemToMeasure ()
		{
			return Task.FromResult (SelectedLineItem);
		}

		public async Task MeasureCurrentLineItemVisually (int fullBottles, ICollection<decimal> partials)
		{
			if(SelectedLineItem == null){
				throw new InvalidOperationException ("No line item being measured currently!");
			}

			if(fullBottles == 0 && partials.Any() == false){
				//we've got zero, what to do?
			}
			//let's calc the total amount as well
			var totalContainers = (decimal)fullBottles;
			if(partials.Any()){
				totalContainers += partials.Sum();
			}

			var totalAmt = SelectedLineItem.Container.AmountContained.Multiply (totalContainers);

			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var realLI = SelectedLineItem as InventoryBeverageLineItem;

			//make an event
			var ev = _eventFactory.CreateInventoryLiquidItemMeasuredEvent(emp, SelectedInventory, _selectedInventorySection, realLI, fullBottles, partials, totalAmt);

			SelectedInventory = _inventoryRepository.ApplyEvent (ev);
			SelectedLineItem = null;
		}

	    public Task<IInventorySection> GetCurrentInventorySection()
	    {
	        return Task.FromResult(_selectedInventorySection);
	    }

	    public async Task AddNewSection (string sectionName, bool hasPartialBottles, bool isDefaultInventorySection)
		{
			await _loginService.AddNewSectionToRestaurant (sectionName, hasPartialBottles, isDefaultInventorySection);

			//do we have an inventory that needs to get the new section added?
			if(SelectedInventory != null){
				var existingSec = SelectedInventory.GetSections ().FirstOrDefault (invS => invS.Name == sectionName);
				if(existingSec == null){
					var rest = await _loginService.GetCurrentRestaurant ();
					if (rest != null) {
						var existingRestaurantSec = rest.GetInventorySections ().FirstOrDefault (rs => rs.Name == sectionName);
						if (existingRestaurantSec != null) {
							var emp = await _loginService.GetCurrentEmployee ();
							var invEv = _eventFactory.CreateInventoryNewSectionAddedEvent (emp, SelectedInventory, existingRestaurantSec);
							SelectedInventory = _inventoryRepository.ApplyEvent (invEv);
							await _inventoryRepository.Commit(SelectedInventory.ID);
						}
					}
				}
			}
				
		}

	    public async Task<IEnumerable<IInventory>> GetCompletedInventoriesForCurrentRestaurant(DateTimeOffset? start, DateTimeOffset? end)
	    {
	        var currentRestaurant = await _loginService.GetCurrentRestaurant();
	        if (currentRestaurant == null)
	        {
	            throw new InvalidOperationException("No current restaurant");
	        }

	        var items =
	            _inventoryRepository.GetAll()
	                .Where(i => i.DateCompleted.HasValue && i.RestaurantID == currentRestaurant.ID)
					.ToList();

			if (start.HasValue) {
				items = items.Where (i => i.DateCompleted.Value >= start.Value).ToList();
			}

			if (end.HasValue) {
				items = items.Where (i => i.DateCompleted.Value <= end.Value).ToList();
			}
	        return items;
	    }

	    public Task<IInventory> GetSelectedInventory ()
		{
			return Task.FromResult (SelectedInventory);
		}

		public Task<IInventory> GetLastCompletedInventory ()
		{
			return Task.FromResult (LastCompletedInventory);
		}

		#endregion
	}
}

