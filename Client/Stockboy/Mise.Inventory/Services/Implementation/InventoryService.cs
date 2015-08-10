using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities.Inventory;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Services.UtilityServices;
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

	    private IInventoryBeverageLineItem _selectedLineItem;
	    private IInventory _selectedInventory;
	    private IInventory _lastCompletedInventory;
		private IInventory _firstCompletedInventory;
	    private Guid _selectedInventorySectionID;

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
		    if (restID.HasValue)
		    {
		        _selectedInventory = _inventoryRepository.GetAll()
		            .Where(i => i.RestaurantID == restID.Value)
		            .Where(i => i.DateCompleted.HasValue == false)
		            .OrderByDescending(i => i.LastUpdatedDate)
		            .FirstOrDefault();

				var completeds = _inventoryRepository.GetAll ()
					.Where (i => i.DateCompleted.HasValue)
					.Where (i => i.RestaurantID == restID.Value)
					.OrderBy (i => i.DateCompleted.Value);
				
				_lastCompletedInventory = completeds.LastOrDefault ();

				_firstCompletedInventory = completeds.FirstOrDefault ();
		    }
		    else
		    {
		        _selectedInventory = null;
		        _lastCompletedInventory = null;
		    }

			return Task.FromResult (true);
		}

		public Task<IInventory> GetCurrentInventory ()
		{
			return Task.FromResult (_selectedInventory);
		}

		public async Task<IEnumerable<IInventoryBeverageLineItem>> GetLineItemsForCurrentSection ()
		{
			var rest = await _loginService.GetCurrentRestaurant ();
			var inv = await _inventoryRepository.GetCurrentInventory (rest.ID);

			if (inv == null) {
				throw new InvalidOperationException ("No current inventory to get line items for!");
			}

			var invSection = inv.GetSections ().FirstOrDefault (sec => sec.ID == _selectedInventorySectionID);
			if (invSection != null) {
				return invSection.GetInventoryBeverageLineItemsInSection ();
			}

		    throw new InvalidOperationException ("No current inventory section!");
		}

		public async Task StartNewInventory ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var rest = await _loginService.GetCurrentRestaurant ();

			var createEvent = _eventFactory.CreateInventoryCreatedEvent (emp);

			_selectedInventory = _inventoryRepository.ApplyEvent (createEvent);

            //create the sections
			var restaurantSections = rest.GetInventorySections();
		    var sectionEvents = restaurantSections
				.Select(rs => _eventFactory.CreateInventoryNewSectionAddedEvent(emp, _selectedInventory, rs)).ToList();
			if (sectionEvents.Any ()) {
				_selectedInventory = _inventoryRepository.ApplyEvents (sectionEvents);
			}

			var currEv = _eventFactory.CreateInventoryMadeCurrentEvent (emp, _selectedInventory);

			_selectedInventory = _inventoryRepository.ApplyEvent (currEv);

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
		                    _selectedInventory.GetSections()
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
		                                newSection, li.InventoryPosition, _selectedInventory);
		                            events.Add(ev);
		                        }
		                    }
		                }
		            }

		            if (events.Any())
		            {
		                _selectedInventory = _inventoryRepository.ApplyEvents(events);
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
				await _inventoryRepository.Commit (_selectedInventory.ID);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (string name, ICategory category,
			string upc, int quantity, int caseSize, LiquidContainer container, Money pricePaid)
		{
			var emp = await _loginService.GetCurrentEmployee ();


			var categories = new []{ category as ItemCategory };
			var addEv = _eventFactory.CreateInventoryLineItemAddedEvent (emp, name, upc, categories, caseSize, container, quantity, pricePaid, null, GetSelectedSection (), 
				GetSelectedSection ().GetNextItemPosition(), _selectedInventory);
		
			_selectedInventory = _inventoryRepository.ApplyEvent (addEv);
            ReportNumItemsInTransaction();

			return _selectedInventory.GetBeverageLineItems ().FirstOrDefault (li => li.ID == addEv.LineItemID);
		}

		public async Task<IInventoryBeverageLineItem> AddLineItemToCurrentInventory (IBaseBeverageLineItem source, int quantity, Money pricePaid)
		{
			if(_selectedInventory == null){
				throw new InvalidOperationException ("Cannot add line item without selected inventory!");
			}
			var emp = await _loginService.GetCurrentEmployee ();

			var addEv = _eventFactory.CreateInventoryLineItemAddedEvent (emp, source, quantity, pricePaid, null, GetSelectedSection (), GetSelectedSection ().GetNextItemPosition(), _selectedInventory);
		
			_selectedInventory = _inventoryRepository.ApplyEvent (addEv);
            ReportNumItemsInTransaction();

			return _selectedInventory.GetBeverageLineItems ().FirstOrDefault (li => li.ID == addEv.LineItemID);
		}

	    public Task SetCurrentInventorySection(IInventorySection section)
	    {
	        _selectedInventorySectionID = section.ID;
	        return Task.FromResult(true);
	    }

	    public async Task MarkSectionAsComplete ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);

			var compEv = _eventFactory.CreateInventorySectionCompletedEvent (emp, _selectedInventory, GetSelectedSection ());

			_selectedInventory = _inventoryRepository.ApplyEvent (compEv);

            //let's commit here to reduce transaction size
			 await _inventoryRepository.Commit(_selectedInventory.ID).ConfigureAwait(false);
		}

		public async Task MarkInventoryAsComplete ()
		{
			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var compEv = _eventFactory.CreateInventoryCompletedEvent (emp, _selectedInventory);

			_inventoryRepository.ApplyEvent (compEv);

			await _inventoryRepository.Commit (_selectedInventory.ID).ConfigureAwait (false);

			//TODO our current should be copied over from here
			_insights.Track("Completed Inventory", "Inventory ID", _selectedInventory.ID.ToString ());
			_lastCompletedInventory = _selectedInventory;
			_selectedInventory = null;
		}

		public Task MarkLineItemForMeasurement (IInventoryBeverageLineItem li)
		{
			_selectedLineItem = li;
			return Task.FromResult (false);
		}

		public Task<IInventoryBeverageLineItem> GetLineItemToMeasure ()
		{
			return Task.FromResult (_selectedLineItem);
		}

		public async Task MeasureCurrentLineItemVisually (int fullBottles, ICollection<decimal> partials)
		{
			if(_selectedLineItem == null){
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

			var totalAmt = _selectedLineItem.Container.AmountContained.Multiply (totalContainers);

			var emp = await _loginService.GetCurrentEmployee ().ConfigureAwait (false);
			var realLI = _selectedLineItem as InventoryBeverageLineItem;

			//make an event
			var ev = _eventFactory.CreateInventoryLiquidItemMeasuredEvent(emp, _selectedInventory, GetSelectedSection (), realLI, fullBottles, partials, totalAmt);

			_selectedInventory = _inventoryRepository.ApplyEvent (ev);

            ReportNumItemsInTransaction();

			_selectedLineItem = null;
		}

	    public Task<IInventorySection> GetCurrentInventorySection()
	    {
	        return Task.FromResult(GetSelectedSection ());
	    }

	    public async Task AddNewSection (string sectionName, bool hasPartialBottles, bool isDefaultInventorySection)
		{
			await _loginService.AddNewSectionToRestaurant (sectionName, hasPartialBottles, isDefaultInventorySection);

			//do we have an inventory that needs to get the new section added?
			if(_selectedInventory != null){
				var existingSec = _selectedInventory.GetSections ().FirstOrDefault (invS => invS.Name == sectionName);
				if(existingSec == null){
					var rest = await _loginService.GetCurrentRestaurant ();
					if (rest != null) {
						var existingRestaurantSec = rest.GetInventorySections ().FirstOrDefault (rs => rs.Name == sectionName);
						if (existingRestaurantSec != null) {
							var emp = await _loginService.GetCurrentEmployee ();
							var invEv = _eventFactory.CreateInventoryNewSectionAddedEvent (emp, _selectedInventory, existingRestaurantSec);
							_selectedInventory = _inventoryRepository.ApplyEvent (invEv);
							await _inventoryRepository.Commit(_selectedInventory.ID);
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
			return Task.FromResult (_selectedInventory);
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
	        if (_selectedInventory != null)
	        {
	            var numItems = _inventoryRepository.GetNumberOfEventsInTransacitonForEntity(_selectedInventory.ID);
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
			if(_selectedInventory  == null){
				return null;
			}

			return _selectedInventory.GetSections ().FirstOrDefault (sec => sec.ID == _selectedInventorySectionID);
		}
	}
}

