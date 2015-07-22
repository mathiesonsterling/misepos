using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Mise.Core.Repositories;
using Mise.Core.Common.Events;
using Mise.Core.Services.WebServices;


namespace Mise.Inventory
{
	public class RepositoryLoader : IRepositoryLoader
	{
	    private readonly IEmployeeRepository _employeeRepository;
	    private readonly IApplicationInvitationRepository _applicationInvitationRepository;
	    private readonly IVendorRepository _vendorRepository;
	    private readonly IInventoryAppEventFactory _inventoryAppEventFactory;
	    private readonly IRestaurantRepository _restaurantRepository;
	    private readonly IPARRepository _parRepository;
	    private readonly IInventoryRepository _inventoryRepository;
	    private readonly IReceivingOrderRepository _receivingOrderRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

	    public RepositoryLoader(IEmployeeRepository employeeRepository, IApplicationInvitationRepository applicationInvitationRepository, IVendorRepository vendorRepository, IInventoryAppEventFactory inventoryAppEventFactory, IRestaurantRepository restaurantRepository, IPARRepository parRepository, IInventoryRepository inventoryRepository, IReceivingOrderRepository receivingOrderRepository, IPurchaseOrderRepository purchaseOrderRepository)
	    {
	        _employeeRepository = employeeRepository;
	        _applicationInvitationRepository = applicationInvitationRepository;
	        _vendorRepository = vendorRepository;
	        _inventoryAppEventFactory = inventoryAppEventFactory;
	        _restaurantRepository = restaurantRepository;
	        _parRepository = parRepository;
	        _inventoryRepository = inventoryRepository;
	        _receivingOrderRepository = receivingOrderRepository;
	        _purchaseOrderRepository = purchaseOrderRepository;
	    }

	    public async Task LoadRepositories(Guid? restaurantID)
		{
			var repositories = new List<IRepository> {_vendorRepository};

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
			    repositories.Add(_employeeRepository);
                repositories.Add(_applicationInvitationRepository);
				repositories.Add(_parRepository);
				repositories.Add(_inventoryRepository);
				repositories.Add(_receivingOrderRepository);
				repositories.Add(_purchaseOrderRepository);
			}
				
			foreach (var repository in repositories) {
				await repository.Load(restaurantID);
			}

			//get the last ID on each
			var lastIDs = repositories.Select(r => r.GetLastEventID());

			eventFactory.SetLastEventID(lastIDs);
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
	}
}

