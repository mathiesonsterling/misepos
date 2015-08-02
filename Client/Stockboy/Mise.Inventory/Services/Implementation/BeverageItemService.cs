using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events;

using Mise.Core.ValueItems.Inventory;
using Mise.Inventory.Services;
using Mise.Inventory.Services.Implementation;
using Autofac.Core;


namespace Mise.Inventory
{
	public class BeverageItemService : IBeverageItemService
	{
		//TODO change this to use services rather that repositories!
		readonly IVendorRepository _vendorRepository;
		readonly IInventoryRepository _inventoryRepository;
		readonly IParRepository _parRepository;
		readonly IReceivingOrderRepository _roRepository;
   		readonly ILogger _logger;
		readonly IDeviceLocationService _deviceLocationService;
		readonly ILoginService _loginService;

		class ContainerAndTimes{
			public ContainerAndTimes(LiquidContainer container, int times){
				Container = container;
				TimesUsed = times;
			}

			public LiquidContainer Container{get;set;}
			public int TimesUsed{get;set;}
		}

		/// <summary>
		/// Stores how often our container is used, so we present items in that order
		/// </summary>
		readonly Dictionary<string, ContainerAndTimes> _containersAndTimeUsed;
		public BeverageItemService (
			ILogger logger, 
			IDeviceLocationService deviceLocationService,
			IVendorRepository vendorRepository, 
			IParRepository parRepository, 
			IInventoryRepository inventoryRepository,
			IReceivingOrderRepository roRepository,
			ILoginService loginService
		)
		{
			_logger = logger;
			_deviceLocationService = deviceLocationService;
			_vendorRepository = vendorRepository;
			_inventoryRepository = inventoryRepository;
			_parRepository = parRepository;
			_loginService = loginService;
			_roRepository = roRepository;
			_containersAndTimeUsed = GetDefaultContainers ();
		}

		/// <summary>
		/// Gets our most typical containers
		/// </summary>
		/// <returns>The default containers.</returns>
		private Dictionary<string, ContainerAndTimes> GetDefaultContainers()
		{
		    var defaults = LiquidContainer.GetStandardBarSizes();

		    var res = new Dictionary<string, ContainerAndTimes>();
		    foreach (var d in defaults.Where(d => res.ContainsKey(d.DisplayName) == false))
		    {
		        res.Add(d.DisplayName, new ContainerAndTimes(d, 0));
		    }
		    return res;
		}
		#region IBeverageItemService implementation

		public Task<IEnumerable<IBaseBeverageLineItem>> GetPossibleItems (int maxItems = int.MaxValue)
		{
			return GetItemsFromRepositories (string.Empty, maxItems);
		}

		public async Task ExpandVendorSearchRegion (Distance newMaxDistance)
		{
			var location = await _deviceLocationService.GetDeviceLocation ().ConfigureAwait (false);
			await _vendorRepository.GetVendorsWithinRadius (newMaxDistance, location, 100);
		}

		public Task<IEnumerable<IBaseBeverageLineItem>> FindItem (string searchString, int maxItems = int.MaxValue)
		{
			return GetItemsFromRepositories (searchString, maxItems);
		}

		/// <summary>
		/// Gets the items in the correct order
		/// </summary>
		/// <returns>The items from repositories.</returns>
		/// <param name="searchString">Search string.</param>
		/// <param name = "maxItems"></param>
		async Task<IEnumerable<IBaseBeverageLineItem>> GetItemsFromRepositories(string searchString, int maxItems){
			//get the items
			var items = new List<IBaseBeverageLineItem> ();

			var restaurant = await _loginService.GetCurrentRestaurant ();
			if(restaurant == null)
			{
				_logger.Warn ("Call for items without a restaurant set");
				return items;
			}

			try
			{
				//var deviceLocation = await _deviceLocationService.GetDeviceLocation();

				var inventories = _inventoryRepository.GetAll ();
				var pars = _parRepository.GetAll();
				var vendorsWeHave = _vendorRepository.GetAll();
				var allROs = _roRepository.GetAll ();
				//var vendorsAround = await _vendorRepository.GetVendorsNotAssociatedWithRestaurantWithinRadius(restaurant.ID, deviceLocation);

				if (pars != null) {
					items.AddRange (pars.SelectMany(p => p.GetBeverageLineItems()));
				}

				var roItems = allROs.SelectMany (ro => ro.GetBeverageLineItems ());
				items = AddInOrderIfNotAlreadyPresent (items, roItems.ToList<IBaseBeverageLineItem>());

				var oldInvItems = inventories.SelectMany (i => i.GetBeverageLineItems ());
				items = AddInOrderIfNotAlreadyPresent (items, oldInvItems);

				var ourVendorItems = vendorsWeHave.SelectMany (v => v.GetItemsVendorSells());
				items = AddInOrderIfNotAlreadyPresent (items, ourVendorItems);

				var filtItems = string.IsNullOrEmpty (searchString) ? items.ToList () : items.Where(i => i.ContainsSearchString (searchString)).ToList ();

				//update our containers here!
				foreach (var container in filtItems.Select(item => item.Container))
				{
				    if(_containersAndTimeUsed.ContainsKey (container.DisplayName) == false){
						_containersAndTimeUsed.Add(container.DisplayName, new ContainerAndTimes (container, 1));
				    } else{
				        var tuple = _containersAndTimeUsed [container.DisplayName];
						tuple.TimesUsed++;
				    }
				}
				return filtItems
					.OrderBy (i => i.DisplayName)
					.Take (maxItems);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		static List<IBaseBeverageLineItem> AddInOrderIfNotAlreadyPresent(IList<IBaseBeverageLineItem> mainList, 
			IEnumerable<IBaseBeverageLineItem> toAdd){
			var resList = new List<IBaseBeverageLineItem> (mainList);

			//which items in the new list do we not already have?
			foreach(var newItem in toAdd.OrderBy (i => i.DisplayName)){
				var exists = resList.Any (oldItem => BeverageLineItemEquator.AreSameBeverageLineItem (oldItem, newItem));
				if(exists == false){
					resList.Add (newItem);
				}
			}

			return resList;
		}

		public Task<IEnumerable<LiquidContainer>> GetAllPossibleContainerSizes ()
		{
			var containers = _containersAndTimeUsed.Values.AsEnumerable ()
				.OrderByDescending (t => t.TimesUsed)
				.ThenBy (t => t.Container.DisplayName)
				.Select (t => t.Container);
			return Task.FromResult (containers);
		}
			
		#endregion
	}
}

