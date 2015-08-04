using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Base;

using Microsoft.WindowsAzure.MobileServices;
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
using Mise.Core.Common.Entities.DTOs.AzureTypes;
using Mise.Core.Common.Events.DTOs.AzureTypes;
using Mise.Core.Common.Services.WebServices;
using Mise.Inventory.ViewModels;
using Mise.Inventory.Services.Implementation.WebServiceClients.Exceptions;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	/// <summary>
	/// Azure Mobile Services client - all items are serialized as JSON before going to the server
	/// </summary>
	public class AzureWeakTypeSharedClient : IInventoryApplicationWebService
	{
		private readonly IMobileServiceClient _client;
		readonly ILogger _logger;
		private readonly IJSONSerializer _serial;
		private readonly BuildLevel _level;
		private readonly EventDataTransportObjectFactory _eventDTOFactory;
		private readonly EntityDataTransportObjectFactory _entityDTOFactory;
		public AzureWeakTypeSharedClient (ILogger logger, IJSONSerializer serializer, IMobileServiceClient client, BuildLevel level)
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

		public async Task<bool> SendEventsAsync (RestaurantAccount updatedEntity, IEnumerable<IAccountEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (ApplicationInvitation updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IApplicationInvitationEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity(updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (PurchaseOrder updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IPurchaseOrderEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (Restaurant updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IRestaurantEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity(updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (ReceivingOrder updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IReceivingOrderEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		public async Task<bool> SendEventsAsync (Core.Common.Entities.Inventory.Inventory updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IInventoryEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}
		#endregion

		#region IApplicationInvitationWebService implementation

		public async Task<IEnumerable<ApplicationInvitation>> GetInvitationsForRestaurant (Guid restaurantID)
		{
			var reals = await GetEntityOfTypeForRestaurant<ApplicationInvitation> (restaurantID);
			return reals;
		}

		public async Task<IEnumerable<ApplicationInvitation>> GetInvitationsForEmail (EmailAddress email)
		{
            var type = typeof(ApplicationInvitation);

		    var table = GetEntityTable();

			var storageItems = await table
				.Where (si => si.MiseEntityType == type.ToString ())
				.ToEnumerableAsync ();

			var realItems = storageItems
				.Select(si => si.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<ApplicationInvitation> (dto));
			return realItems.Where (ai => ai.DestinationEmail != null && ai.DestinationEmail.Equals (email));
		}

		#endregion



		#region IReceivingOrderWebService implementation

		public async Task<IEnumerable<ReceivingOrder>> GetReceivingOrdersForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<ReceivingOrder> (restaurantID);
		    return items;
		}

		#endregion


		#region IInventoryWebService implementation

		public async Task<IEnumerable<Core.Common.Entities.Inventory.Inventory>> GetInventoriesForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<Mise.Core.Common.Entities.Inventory.Inventory> (restaurantID);
		    return items;
		}

		#endregion



		#region IPARWebService implementation

		public async Task<Par> GetCurrentPAR (Guid restaurantID)
		{
			var allPars = await GetPARsForRestaurant (restaurantID);
			return allPars.FirstOrDefault (p => p.IsCurrent);
		}

		public async Task<IEnumerable<Par>> GetPARsForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<Par> (restaurantID);
			return items;
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (Par updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IParEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		#endregion

		#region IVendorWebService implementation

		public async Task<IEnumerable<Vendor>> GetVendorsWithinSearchRadius (Location currentLocation, Distance radius)
		{

		    var table = GetEntityTable();

			var vendType = typeof(Vendor);
			var ais = await table.Where (ai => ai.MiseEntityType == vendType.ToString ()).ToEnumerableAsync ();

			//todo figure out a better way to do this on the server
			var vendors = ais.Select(ai => ai.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<Vendor> (dto));

			return vendors;
				/*.Where(v => v.StreetAddress != null && v.StreetAddress.StreetAddressNumber != null)
				.Select(
					v => new
					{
						DistanceFromPoint = new Distance(currentLocation, v.StreetAddress.StreetAddressNumber),
						Vendor = v
					})
				.Where(vl => radius.GreaterThan(vl.DistanceFromPoint))
				.OrderBy(vl => vl.DistanceFromPoint)
				.Select(vl => vl.Vendor);*/
		}

		public async Task<IEnumerable<Vendor>> GetVendorsAssociatedWithRestaurant (Guid restaurantID)
		{
		    var table = GetEntityTable();

			var vendType = typeof(Vendor);
			var ais = await table.Where (ai => ai.MiseEntityType == vendType.ToString ()).ToEnumerableAsync ();

			//todo figure out a better way to do this on the server
			var vendors = ais.Select(ai => ai.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<Vendor> (dto));

			return vendors.Where(v => v.GetRestaurantIDsAssociatedWithVendor ().Contains (restaurantID));
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (Vendor updatedEntity, IEnumerable<Mise.Core.Entities.Vendors.Events.IVendorEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity<Vendor> (updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		#endregion

		#region IInventoryRestaurantWebService implementation

		public async Task<IEnumerable<Restaurant>> GetRestaurants (Location deviceLocation, Distance maxDistance)
		{
			var restType = typeof(Restaurant).ToString ();
		    var table = GetEntityTable();

			var azureItems = await table.Where (ai => ai.MiseEntityType == restType).ToEnumerableAsync ();

			var rests = azureItems.Select (ai => ai.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<Restaurant> (dto));

			return rests;
				/*.Where(r => r.StreetAddress != null && r.StreetAddress.StreetAddressNumber != null)
				.Select(
					v => new
					{
						DistanceFromPoint = new Distance(deviceLocation, v.StreetAddress.StreetAddressNumber),
						Restaurant = v
					})
				.Where(vl => maxDistance.GreaterThan(vl.DistanceFromPoint))
				.OrderBy(vl => vl.DistanceFromPoint)
				.Select(vl => vl.Restaurant);*/
		}

		public async Task<Restaurant> GetRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<Restaurant> (restaurantID);
			return items.FirstOrDefault ();
		}

		#endregion



		#region IInventoryEmployeeWebService implementation

		public async Task<IEnumerable<Employee>> GetEmployeesAsync ()
		{
			var empType = typeof(Employee).ToString ();

		    var table = GetEntityTable();

			var ais = await table.Where (ai => ai.MiseEntityType == empType).ToEnumerableAsync ();

			return ais.Select (ai => ai.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<Employee> (dto));
		}

		public async Task<IEnumerable<Employee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			var items = await GetEntityOfTypeForRestaurant<Employee> (restaurantID);
			return items;
		}

		public async Task<Employee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			var items = (await GetEmployeesAsync ()).ToList();
				var found = items.FirstOrDefault (e => e.PrimaryEmail != null && e.PrimaryEmail.Equals (email)
			&& e.Password != null && e.Password.Equals (password));

			if(found == null){
				if(items.Any(e => e.PrimaryEmail != null && e.PrimaryEmail.Equals (email))){
					throw new UserNotFoundException (email, false, true);
				} else{
					throw new UserNotFoundException (email);
				}
			} 

			return found;
		}

		#endregion

		#region IEventStoreWebService implementation

		public async Task<bool> SendEventsAsync (Employee updatedEntity, IEnumerable<Mise.Core.Entities.People.Events.IEmployeeEvent> events)
		{
			var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();

			var entRes = await StoreEntity(updatedEntity);
			var evRes = await SendEventDTOs (dtos);

			return entRes && evRes;
		}

		#endregion
		#endregion

	
		private async Task<bool> SendEventDTOs(ICollection<EventDataTransportObject> dtos){
	
			var table = GetEventTable ();
			var existingIDs = await table.Select (ai => ai.EventID).ToCollectionAsync ();

			//get those that exist and those that don't
			var news = dtos.Where (dto => existingIDs.Contains (dto.ID) == false);
			var createTasks = news
				.Select(dto => new AzureEventStorage (dto))
				.Select (si => table.InsertAsync (si));

			var existing = dtos.Where (dto => existingIDs.Contains (dto.ID));
			var updateTasks = existing.Select (dto => new AzureEventStorage (dto))
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

		private async Task<bool> StoreEntity<TRealType>(TRealType entity) where TRealType : IEntityBase, new()
		{
			var dto = _entityDTOFactory.ToDataTransportObject (entity);

			var storageItem = new AzureEntityStorage (dto);

			var table = GetEntityTable();

			//does this exist?
			var exists = (await table.Where(ai => ai.EntityID == dto.ID).ToEnumerableAsync ()).Any();
			if(exists){
				await table.UpdateAsync (storageItem);
			} else {
				await table.InsertAsync (storageItem);
			}

			return true;
		}

	    private IMobileServiceTable<AzureEntityStorage> GetEntityTable() 
	    {
	        return _client.GetTable<AzureEntityStorage>();
	    }

		private IMobileServiceTable<AzureEventStorage> GetEventTable ()
		{
			return _client.GetTable<AzureEventStorage> ();
		}

	    private async Task<IEnumerable<T>> GetEntityOfTypeForRestaurant<T>(Guid restaurantID) where T:class, IEntityBase, new()
		{
			var type = typeof(T).ToString ();

	        var table = GetEntityTable();

			var storageItems = await table
				.Where (si => si.MiseEntityType == type && si.RestaurantID.HasValue && si.RestaurantID == restaurantID)
				.ToEnumerableAsync ();

			var realItems = storageItems
				.Select(si => si.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<T> (dto));
			return realItems;
		}
	}
}

