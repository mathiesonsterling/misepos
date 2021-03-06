﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Client.Services;



namespace Mise.Inventory.Services.Implementation
{
    public class InventoryService : IInventoryService
	{
		readonly IInventoryRepository _inventoryRepository;
		readonly ILoginService _loginService;
		readonly IInventoryAppEventFactory _eventFactory;
		readonly ILogger _logger;
	    readonly IInsightsService _insights;

	    private Guid? _selectedLineItemId;
	    private Guid? _selectedInventoryID;
	    private IInventory _lastCompletedInventory;
		private IInventory _firstCompletedInventory;
	    private Guid _selectedInventorySectionID;

		public InventoryService (ILogger logger, ILoginService loginService, 
			IInventoryRepository inventoryRespository, IInventoryAppEventFactory eventFactory, 
            IInsightsService insightsService)
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
		    if (restID.HasValue)
		    {
				//TODO allow us to load up inventories from other devices ONLY if our restaurant supports it
				//via billing feature
				_selectedInventoryID = null;

				var completeds = _inventoryRepository.GetAll ()
					.Where (i => i.DateCompleted.HasValue)
					.Where (i => i.RestaurantID == restID.Value)
					.OrderBy (i => i.DateCompleted.Value);
				
				_lastCompletedInventory = completeds.LastOrDefault ();

				_firstCompletedInventory = completeds.FirstOrDefault ();
		    }
		    else
		    {
		        _selectedInventoryID = null;
		        _lastCompletedInventory = null;
		    }

			return Task.FromResult (true);
		}

		public Task<IInventory> GetCurrentInventory ()
		{
			IInventory inv = null;
			if(_selectedInventoryID.HasValue){
				inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			}
			return Task.FromResult (inv);
		}

	    public Task SetCurrentInventory(IInventory inventory)
	    {
	        _selectedInventoryID = inventory.Id;
	        return Task.FromResult(false);
	    }

		public Task<IEnumerable<IInventoryBeverageLineItem>> GetLineItemsForCurrentSection ()
		{
			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);

			if (inv == null) {
				throw new InvalidOperationException ("No current inventory to get line items for!");
			}

			var invSection = inv.GetSections ().FirstOrDefault (sec => sec.Id == _selectedInventorySectionID);
			if (invSection != null) {
				return Task.FromResult(invSection.GetInventoryBeverageLineItemsInSection ());
			}

		    throw new InvalidOperationException ("No current inventory section!");
		}

		public async Task StartNewInventory ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var rest = await _loginService.GetCurrentRestaurant ();

			var createEvent = _eventFactory.CreateInventoryCreatedEvent (emp);

			var inv = _inventoryRepository.ApplyEvent (createEvent);

            //create the sections
			var restaurantSections = rest.GetInventorySections();
		    var sectionEvents = restaurantSections
				.Select(rs => _eventFactory.CreateInventoryNewSectionAddedEvent(emp, inv, rs)).ToList();
			if (sectionEvents.Any ()) {
				inv = _inventoryRepository.ApplyEvents (sectionEvents);
			}

			var currEv = _eventFactory.CreateInventoryMadeCurrentEvent (emp, inv);

			inv = _inventoryRepository.ApplyEvent (currEv);

			//do we have a previous inventory?  if so, take the LIs from there
			var previous = _inventoryRepository.GetAll ()
				.Where(i => i.RestaurantID == rest.Id)
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
		                    inv.GetSections()
		                        .FirstOrDefault(newSec =>
		                                newSec.RestaurantInventorySectionID == oldInventorySection.RestaurantInventorySectionID);
		                if (newSection != null)
		                {
		                    var lisInSection = oldInventorySection.GetInventoryBeverageLineItemsInSection().ToList();
	                        foreach (var li in lisInSection)
	                        {
	                            var ev = _eventFactory.CreateInventoryLineItemAddedEvent(emp,
	                                li, 0, li.VendorBoughtFrom,
                                    newSection, li.InventoryPosition, inv, li.PricePaid);
	                            events.Add(ev);
	                        }
		                }
		            }

		            if (events.Any())
		            {
		                inv = _inventoryRepository.ApplyEvents(events);
                        ReportNumItemsInTransaction();
		            }
		        }
		        catch (Exception e)
		        {
		            _logger.HandleException(e);
		            throw;
		        }
		    }
			try{
				await _inventoryRepository.Commit (inv.Id);
				_selectedInventoryID = inv.Id;
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (string name, ICategory category,
			string upc, int quantity, int caseSize, LiquidContainer container, int? inventoryPosition)
		{
			var emp = await _loginService.GetCurrentEmployee ();

			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);

			//TODO - if we've waited too long between items, autosave
			var categories = new []{ category as InventoryCategory };

			var section = GetSelectedSection ();
			var itemPosition = inventoryPosition.HasValue ? inventoryPosition.Value : section.GetNextItemPosition ();
			var addEv = _eventFactory.CreateInventoryLineItemAddedEvent (emp, name, upc, categories, caseSize, container, quantity, null, 
				section, itemPosition, inv);
		
			_inventoryRepository.ApplyEvent (addEv);
            ReportNumItemsInTransaction();

			//make the new item selected
			_selectedLineItemId = addEv.LineItemID;

			return GetSelectedLineItem ();
		}

		public async Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (IBaseBeverageLineItem source, 
			int quantity, int? inventoryPosition)
		{
			
			if(_selectedInventoryID == null){
				throw new InvalidOperationException ("Cannot add line item without selected inventory!");
			}
			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var emp = await _loginService.GetCurrentEmployee ();

			var section = GetSelectedSection ();
			var itemPosition = inventoryPosition.HasValue ? inventoryPosition.Value : section.GetNextItemPosition ();
			var addEv = _eventFactory.CreateInventoryLineItemAddedEvent (emp, source, quantity, null, section,
				itemPosition, inv, null);
		
			_inventoryRepository.ApplyEvent (addEv);
            ReportNumItemsInTransaction();

			//make the new item selected
			_selectedLineItemId = addEv.LineItemID;
			return GetSelectedLineItem ();
		}

	    public async Task SetCurrentInventorySection(IInventorySection section)
	    {
	        _selectedInventorySectionID = section.Id;

			//mark that we've started it
			var emp = await _loginService.GetCurrentEmployee ();
			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var ev = _eventFactory.CreateInventorySectionStartedByEmployeeEvent (emp, inv, section);
			_inventoryRepository.ApplyEvent (ev);

			//TODO send a message also that we've started it so other devices are notified
	    }

	    public async Task MarkSectionAsComplete ()
		{
			//TODO check if we have any inventory positions that aren't multiples of 10
			//if we do, take the items in order and redo them
			var section = GetSelectedSection ();
			section.UpdatePositions ();
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var compEv = _eventFactory.CreateInventorySectionCompletedEvent (emp, inv, section);

			inv = _inventoryRepository.ApplyEvent (compEv);

            //let's commit here to reduce transaction size
			await SaveInventory();
		}

		public async Task SaveInventory ()
		{
            //only save if dirty
            if (_inventoryRepository.Dirty)
            {
                var currInventory = _inventoryRepository.GetByID(_selectedInventoryID.Value);
                await _inventoryRepository.Commit(currInventory.Id).ConfigureAwait(false);
            }
		}

		public async Task ClearCurrentSection ()
		{
			var emp = await _loginService.GetCurrentEmployee ();
			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var sec = GetSelectedSection ();

			var ev = _eventFactory.CreateInventorySectionClearedEvent (emp, inv, sec);

			_inventoryRepository.ApplyEvent (ev);
		}

		public async Task MarkInventoryAsComplete ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var compEv = _eventFactory.CreateInventoryCompletedEvent (emp, inv);

			_inventoryRepository.ApplyEvent (compEv);

			await SaveInventory ();

			_insights.Track("Completed Inventory", "Inventory ID", inv.Id.ToString ());
			_lastCompletedInventory = inv;
			_selectedInventoryID = null;
		}

		public Task MarkLineItemForMeasurement (IInventoryBeverageLineItem li)
		{
			_selectedLineItemId = li.Id;
			return Task.FromResult (false);
		}

		public Task<IInventoryBeverageLineItem> GetLineItemToMeasure ()
		{
			return Task.FromResult (GetSelectedLineItem ());
		}

		public async Task MeasureCurrentLineItemVisually (int fullBottles, ICollection<decimal> partials)
		{
			var selectedLineItem = GetSelectedLineItem ();
			if(selectedLineItem == null){
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

			var totalAmt = selectedLineItem.Container.AmountContained.Multiply (totalContainers);

			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var realLI = selectedLineItem as InventoryBeverageLineItem;

			//make an event
			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var ev = _eventFactory.CreateInventoryLiquidItemMeasuredEvent(emp, inv, GetSelectedSection (), 
                realLI, fullBottles, partials, (LiquidAmount)totalAmt);

			_inventoryRepository.ApplyEvent (ev);

            ReportNumItemsInTransaction();
		}

		public async Task DeleteLineItem (IInventoryBeverageLineItem li)
		{
			var currInv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var currSection = currInv.GetSections ().FirstOrDefault (s => s.Id == _selectedInventorySectionID);
			var emp = await _loginService.GetCurrentEmployee ();

			var ev = _eventFactory.CreateInventoryLineItemDeletedEvent (emp, currInv, currSection, li);

			_inventoryRepository.ApplyEvent (ev);
		}

		public async Task MoveLineItemToPosition (IInventoryBeverageLineItem li, int position)
		{
			var currInv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var currSection = currInv.GetSections ().FirstOrDefault (s => s.Id == _selectedInventorySectionID);
			var emp = await _loginService.GetCurrentEmployee ();

			var ev = _eventFactory.CreateInventoryLineItemMovedToNewPositionEvent (emp, currInv, currSection, li, position);

			_inventoryRepository.ApplyEvent (ev);
		}

		public async Task<int> GetInventoryPositionAfterCurrentItem ()
		{
			var section = GetSelectedSection ();

			//see if the item after next is taken
			var items = section.GetInventoryBeverageLineItemsInSection ()
				.OrderBy (li => li.InventoryPosition)
				.ToList();
			var lineItem = GetSelectedLineItem ();
			var currentItemPos = lineItem.InventoryPosition;
			var nextIndex = currentItemPos + 1;

			var itemToMove = items.FirstOrDefault (li => li.InventoryPosition == nextIndex);
			if(itemToMove != null){
				await MoveLineItemToPosition (itemToMove, nextIndex + 1);
			}

			return nextIndex;
		}

		public Task MoveLineItemUpInList (IInventoryBeverageLineItem li)
		{
			return MoveLineItem (li, true);
		}

		public Task MoveLineItemDownInList (IInventoryBeverageLineItem li)
		{
			return MoveLineItem (li, false);
		}

		async Task MoveLineItem(IInventoryBeverageLineItem lineItem, bool up){
			var emp = await _loginService.GetCurrentEmployee ();
			var currInv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			var currSection = currInv.GetSections ().FirstOrDefault (s => s.Id == _selectedInventorySectionID);

			var items = currSection.GetInventoryBeverageLineItemsInSection().OrderBy (li => li.InventoryPosition).ToList ();
			var currIndex = items.IndexOf (lineItem);

			int newPos;
			if(up){
				if(currIndex < 1){
					return;
				}  
				var prevItem = items[currIndex - 1];
				newPos = prevItem.InventoryPosition - 1;
			} else {
				if(currIndex >= (items.Count - 1)){
					return;
				}
				var nextItem = items[currIndex + 1];
				newPos = nextItem.InventoryPosition + 1;
			}

			var ev = _eventFactory.CreateInventoryLineItemMovedToNewPositionEvent (emp, currInv, currSection, lineItem, newPos);

			_inventoryRepository.ApplyEvent(ev);
		}

	    public Task<IInventorySection> GetCurrentInventorySection()
	    {
	        return Task.FromResult(GetSelectedSection ());
	    }

	    public async Task AddNewSection (string sectionName, bool hasPartialBottles, bool isDefaultInventorySection)
		{
			await _loginService.AddNewSectionToRestaurant (sectionName, hasPartialBottles, isDefaultInventorySection);

			//do we have an inventory that needs to get the new section added?
			if(_selectedInventoryID.HasValue){
				var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
				var existingSec = inv.GetSections ().FirstOrDefault (invS => invS.Name == sectionName);
				if(existingSec == null){
					var rest = await _loginService.GetCurrentRestaurant ();
					if (rest != null) {
						var existingRestaurantSec = rest.GetInventorySections ().FirstOrDefault (rs => rs.Name == sectionName);
						if (existingRestaurantSec != null) {
							var emp = await _loginService.GetCurrentEmployee ();
							var invEv = _eventFactory.CreateInventoryNewSectionAddedEvent (emp, inv, existingRestaurantSec);
							inv = _inventoryRepository.ApplyEvent (invEv);
							await _inventoryRepository.Commit(inv.Id);
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
	                .Where(i => i.DateCompleted.HasValue && i.RestaurantID == currentRestaurant.Id)
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
			IInventory inv = null;
			if(_selectedInventoryID.HasValue){
				inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			}
			return Task.FromResult (inv);
		}

		public Task<IInventory> GetLastCompletedInventory ()
		{
			return Task.FromResult (_lastCompletedInventory);
		}

		public Task<IInventory> GetFirstCompletedInventory ()
		{
			return Task.FromResult (_firstCompletedInventory);
		}

		public Task<bool> HasInventoryPriorToDate (Guid restaurantID, DateTimeOffset date)
		{
			var res = _inventoryRepository.GetAll ()
				.Any(i => i.RestaurantID == restaurantID && i.DateCompleted.HasValue && i.DateCompleted.Value <= date);

			return Task.FromResult (res);
		}

		#endregion

	    private const int REPORT_NUMBER_OF_EVENTS_THRESHOLD = 50;
        /// <summary>
        /// If we have a large number of events waiting for commit, let's mark an event of it.  Later we might do a bleed off
        /// </summary>
	    private void ReportNumItemsInTransaction()
	    {
	        if (_selectedInventoryID.HasValue)
	        {
				var numItems = _inventoryRepository.GetNumberOfEventsInTransacitonForEntity(_selectedInventoryID.Value);
	            if (numItems > REPORT_NUMBER_OF_EVENTS_THRESHOLD)
	            {
	                var insightsParam = new Dictionary<string, string>
	                {
	                    {"Repository", "Inventory"},
	                    {"Number of items", numItems.ToString()}
	                };
                    _insights.Track("Large number of events in repository", insightsParam);
	            }
	        }
	    }

		private IInventorySection GetSelectedSection(){
			//always get off the inventory, to ensure we're getting the latest!
			if(_selectedInventoryID.HasValue == false){
				return null;
			}

			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			return inv.GetSections ().FirstOrDefault (sec => sec.Id == _selectedInventorySectionID);
		}

		IInventoryBeverageLineItem GetSelectedLineItem(){
			if(_selectedInventoryID.HasValue == false){
				return null;
			}

			var inv = _inventoryRepository.GetByID (_selectedInventoryID.Value);
			return inv.GetBeverageLineItems ().FirstOrDefault (li => li.Id == _selectedLineItemId.Value);
		}

        public bool HasCurrentInventoryShownClearReminder()
        {
            if (!_selectedInventoryID.HasValue)
            {
                return false;
            }
            var inv = _inventoryRepository.GetByID(_selectedInventoryID.Value);
            if (inv == null)
            {
                return false;
            }
            return _loginService.HasInventoryShownClearReminder(inv.Id);
        }

        public async Task MarkCurrentInventoryShownClearReminderAsShown()
        {
            if (_selectedInventoryID.HasValue)
            {
                var inv = _inventoryRepository.GetByID(_selectedInventoryID.Value);
                if (inv != null)
                {
                    await _loginService.MarkInventoryShownClearReminderAsShown(inv.Id);
                }
            }
        }
	}
}

