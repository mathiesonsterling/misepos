using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.Client.Services;

namespace Mise.Inventory
{
	public class RepositoryLoader : IRepositoryLoader
	{
		private readonly ILogger _logger;
	    private readonly IEmployeeRepository _employeeRepository;
	    private readonly IApplicationInvitationRepository _applicationInvitationRepository;
	    private readonly IVendorRepository _vendorRepository;
	    private readonly IInventoryAppEventFactory _inventoryAppEventFactory;
	    private readonly IRestaurantRepository _restaurantRepository;
	    private readonly IParRepository _parRepository;
	    private readonly IInventoryRepository _inventoryRepository;
	    private readonly IReceivingOrderRepository _receivingOrderRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
	    private readonly IBeverageItemService _beverageItemService;
		public RepositoryLoader(ILogger logger,
			IEmployeeRepository employeeRepository, IApplicationInvitationRepository applicationInvitationRepository, 
            IVendorRepository vendorRepository, IInventoryAppEventFactory inventoryAppEventFactory, 
			IRestaurantRepository restaurantRepository, 
            IParRepository parRepository, IInventoryRepository inventoryRepository, 
			IReceivingOrderRepository receivingOrderRepository, 
            IPurchaseOrderRepository purchaseOrderRepository, IBeverageItemService beverageItemService)
	    {
			_logger = logger;
	        _employeeRepository = employeeRepository;
	        _applicationInvitationRepository = applicationInvitationRepository;
	        _vendorRepository = vendorRepository;
	        _inventoryAppEventFactory = inventoryAppEventFactory;
	        _restaurantRepository = restaurantRepository;
	        _parRepository = parRepository;
	        _inventoryRepository = inventoryRepository;
	        _receivingOrderRepository = receivingOrderRepository;
	        _purchaseOrderRepository = purchaseOrderRepository;
	        _beverageItemService = beverageItemService;
	    }

	    public async Task LoadRepositories(Guid? restaurantID)
		{
			var repositories = new List<IRepository>();

	        //TODO - load the vendors based on distance once that's available

	        var eventFactory = _inventoryAppEventFactory;

			var restRepos = _restaurantRepository;

	        var needsReload = false;
			try{
				await restRepos.Load (restaurantID);
			} catch(ServerNotReadyException){
				//was it the service starting up?  if so, wait a second then reload
			    needsReload = true;
			}

	        if (needsReload)
	        {
                var reload = Task.Delay(100);
                await reload;
                await restRepos.Load(restaurantID);
	        }

			if (restaurantID.HasValue)
			{
				repositories.Add (_vendorRepository);
			    repositories.Add(_employeeRepository);
                repositories.Add(_applicationInvitationRepository);
				repositories.Add(_parRepository);
				repositories.Add(_inventoryRepository);
				repositories.Add(_receivingOrderRepository);
				repositories.Add(_purchaseOrderRepository);
			}

			var loadTasks = repositories.Select (r => r.Load (restaurantID));
			await Task.WhenAll (loadTasks);

			//get the last ID on each
			var lastIDs = repositories.Select(r => r.GetLastEventID());

			eventFactory.SetLastEventID(lastIDs);

	        await _beverageItemService.ReloadItemCache();
	        //done!
		}

	    public async Task ClearAllRepositories()
	    {
	        await _employeeRepository.Clear();
            await _applicationInvitationRepository.Clear();
	        await _vendorRepository.Clear();
	        await _parRepository.Clear();
	        await _inventoryRepository.Clear();
	        await _receivingOrderRepository.Clear();
	        await _purchaseOrderRepository.Clear();
	    }

		public async Task SaveOnSleep ()
		{
			//we want to attempt all our saves, regardless if the first fails
			List<Task> commits = new List<Task> ();
			Exception e = null;
			if (_inventoryRepository.Dirty) {
				var t= _inventoryRepository.CommitAll ();
				commits.Add(t);
			}
				
			if (_parRepository.Dirty) {
				var t= _parRepository.CommitAll ();
				commits.Add (t);
			}
				
			if (_receivingOrderRepository.Dirty) {
				var t = _receivingOrderRepository.CommitAll ();
				commits.Add (t);
			}

			foreach (var c in commits) {
				try{
					await c.ConfigureAwait (false);
				} catch(Exception ex){
					_logger.HandleException (ex);
					if (e == null) {
						e = ex;
					}
				}
			}

			if (e != null) {
				throw e;
			}
		}
	}
}

