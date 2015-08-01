using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Base;

using Microsoft.WindowsAzure.MobileServices;
using Mise.Core.Services.WebServices;
using Mise.Core.Entities.Accounts;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;

using Mise.Core.ValueItems;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Entities.Accounts;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	/// <summary>
	/// Shared portions of the windows azure client
	/// </summary>
	public class AzureSharedClient : IInventoryApplicationWebService
	{
		private readonly MobileServiceClient _client;
		readonly ILogger _logger;
		private readonly IJSONSerializer _serial;
		private readonly BuildLevel _level;
		private readonly EventDataTransportObjectFactory _eventDTOFactory;
		private readonly EntityDataTransportObjectFactory _entityDTOFactory;
		public AzureSharedClient (ILogger logger, IJSONSerializer serializer, MobileServiceClient client, BuildLevel level)
		{
			_logger = logger;
			_client = client;
			_serial = serializer;
			_level = level;

			_eventDTOFactory = new EventDataTransportObjectFactory (_serial);
			_entityDTOFactory = new EntityDataTransportObjectFactory (_serial);
		}

		#region IInventoryApplicationWebService implementation

		#region IResendEventsWebService implementation

		public Task<bool> ResendEvents (ICollection<IEntityEventBase> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();
			return SendEventDTOs (dtos);
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (IAccount updatedEntity, IEnumerable<IAccountEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<RestaurantAccount, IAccount> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (IApplicationInvitation updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IApplicationInvitationEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<ApplicationInvitation, IApplicationInvitation> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (IPurchaseOrder updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IPurchaseOrderEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<PurchaseOrder, IPurchaseOrder> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (IRestaurant updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IRestaurantEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<Restaurant, IRestaurant> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (IReceivingOrder updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IReceivingOrderEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<ReceivingOrder, IReceivingOrder> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (IInventory updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IInventoryEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<Mise.Core.Common.Entities.Inventory.Inventory, IInventory> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}
		#endregion

		#region IApplicationInvitationWebService implementation

		public async Task<IEnumerable<IApplicationInvitation>> GetInvitationsForRestaurant (Guid restaurantID)
		{
			var reals = await GetEntityOfTypeForRestaurant<ApplicationInvitation> (restaurantID);
			return reals.Cast<IApplicationInvitation> ();
		}

		public async Task<IEnumerable<IApplicationInvitation>> GetInvitationsForEmail (EmailAddress email)
		{			var type = typeof(IApplicationInvitation);

			var table = _client.GetTable<AzureEntityStorage> ();

			var storageItems = await table
				.Where (si => si.MiseEntityType == type.ToString ())
				.ToEnumerableAsync ();

			var realItems = storageItems
				.Select(si => si.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<IApplicationInvitation> (dto));
			return realItems.Where (ai => ai.DestinationEmail != null && ai.DestinationEmail.Equals (email));
		}

		#endregion



		#region IReceivingOrderWebService implementation

		public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<ReceivingOrder> (restaurantID);
			return items.Cast<IReceivingOrder> ();
		}

		#endregion


		#region IInventoryWebService implementation

		public async Task<IEnumerable<IInventory>> GetInventoriesForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<Mise.Core.Common.Entities.Inventory.Inventory> (restaurantID);
			return items.Cast<IInventory> ();
		}

		#endregion



		#region IPARWebService implementation

		public Task<IPAR> GetCurrentPAR (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

		public Task<IEnumerable<IPAR>> GetPARsForRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (IPAR updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IPAREvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<Par, IPAR> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		#endregion

		#region IVendorWebService implementation

		public Task<IEnumerable<IVendor>> GetVendorsWithinSearchRadius (Location currentLocation, Distance radius)
		{
			throw new NotImplementedException ();
		}

		public Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (IVendor updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IVendorEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<Vendor, IVendor> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		#endregion

		#region IInventoryRestaurantWebService implementation

		public Task<IEnumerable<IRestaurant>> GetRestaurants (Location deviceLocation)
		{
			throw new NotImplementedException ();
		}

		public Task<IRestaurant> GetRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

		#endregion



		#region IInventoryEmployeeWebService implementation

		public Task<IEnumerable<IEmployee>> GetEmployeesAsync ()
		{
			throw new NotImplementedException ();
		}

		public async Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<Employee> (restaurantID);
			return items.Cast<IEmployee> ();
		}

		public Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			throw new NotImplementedException ();
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (IEmployee updatedEntity, IEnumerable<Mise.Core.Entities.People.Events.IEmployeeEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<Employee, IEmployee>(updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		#endregion
		#endregion

	
		private async Task<bool> SendEventDTOs(IEnumerable<EventDataTransportObject> dtos){
	
			var table = _client.GetTable<AzureEventStorage> ();
			var existingIDs = await table.Select (ai => ai.EventID).ToCollectionAsync ();

			//get those that exist and those that don't
			var news = dtos.Where (dto => existingIDs.Contains (dto.ID) == false);
			var createTasks = news
				.Select(dto => new AzureEventStorage (dto, _level))
				.Select (si => table.InsertAsync (si));

			var existing = dtos.Where (dto => existingIDs.Contains (dto.ID));
			var updateTasks = existing.Select (dto => new AzureEventStorage (dto, _level))
				.Select (si => table.UpdateAsync (si));
			
			try{
				await Task.WhenAll (createTasks);
				await Task.WhenAll (updateTasks);
				return true;
			} catch(Exception e){
				_logger.HandleException (e);
				return false;
			}
		}

		private async Task<bool> StoreEntity<TRealType, TInterface>(TInterface entity) where TRealType : TInterface, IEntityBase, new()
		{
			TRealType concrete = default(TRealType);
			try{
				concrete = (TRealType)entity;
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Error);
				return false;
			}
			var dto = _entityDTOFactory.ToDataTransportObject (concrete);

			var storageItem = new AzureEntityStorage (dto, _level);

			var table = _client.GetTable<AzureEntityStorage>();

			//does this exist?
			var exists = (await table.Where(ai => ai.EntityID == dto.ID).ToEnumerableAsync ()).Any();
			if(exists){
				await table.UpdateAsync (storageItem);
			} else {
				await table.InsertAsync (storageItem);
			}

			return true;
		}

		private async Task<IEnumerable<T>> GetEntityOfTypeForRestaurant<T>(Guid restaurantID) where T:class, IEntityBase, new()
		{
			var type = typeof(T);

			var table = _client.GetTable<AzureEntityStorage> ();

			var storageItems = await table
				.Where (si => si.MiseEntityType == type.ToString () && si.RestaurantID.HasValue && si.RestaurantID == restaurantID)
				.ToEnumerableAsync ();

			var realItems = storageItems
				.Select(si => si.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<T> (dto));
			return realItems;
		}
	}
}

