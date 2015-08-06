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
		private readonly EventDataTransportObjectFactory _eventDTOFactory;
		private readonly EntityDataTransportObjectFactory _entityDTOFactory;
		public AzureWeakTypeSharedClient (ILogger logger, IJSONSerializer serializer, IMobileServiceClient client)
		{
			_logger = logger;
			_client = client;
			_serial = serializer;
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
			//TODO - when we put this back into main, make it enum, not collection
			var ais = await table.Where (ai => ai.MiseEntityType == vendType.ToString ()).ToCollectionAsync ();

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
			var filtered = vendors.Where(v => v.GetRestaurantIDsAssociatedWithVendor ().Contains (restaurantID)).ToList();

			return filtered;
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
			var type = typeof(Restaurant).ToString ();

			var table = GetEntityTable();

			var storageItems = await table
				.Where (si => si.MiseEntityType == type && si.EntityID == restaurantID)
				.ToEnumerableAsync ();

			var ai = storageItems.FirstOrDefault ();
			if(ai == null){
				return null;
			}
				
			_logger.Debug ("Rehydrating item of type " + type + " and id " + ai.EntityID);
			var dto = ai.ToRestaurantDTO ();
			if(dto == null){
				throw new Exception ("Error turning item " + ai.EntityID + " to RestaurantDTO!");
			} else {
				var real = _entityDTOFactory.FromDataStorageObject<Restaurant> (dto);
				if(real == null){
					throw new Exception ("Error rehydrating item ID " + dto.ID);
				} else {
					return real;
				}
			}
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

			var dtoIDs = dtos.Select (dto => dto.ID).ToList();
			var existingEvents = await table.Where(ev => dtoIDs.Contains (ev.EventID)).ToEnumerableAsync ();

			//get those that exist and those that don't
			var createTasks = dtos
				.Select(dto => new AzureEventStorage (dto))
				.Select (si => table.InsertAsync (si));

			var delTasks = existingEvents.Select (ev => table.DeleteAsync (ev));
			
			try{
				await Task.WhenAll (delTasks);
				await Task.WhenAll (createTasks);
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
			var existing = (await table.Where(ai => ai.EntityID == dto.ID).ToEnumerableAsync ()).FirstOrDefault ();
			if(existing != null){
				//update is not firing, do NOT know why, but we'll do this in the meantime
				//await table.UpdateAsync (storageItem);
				await table.DeleteAsync (existing);
			} 
			await table.InsertAsync (storageItem);

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
				.Where (si => si.MiseEntityType == type && si.RestaurantID != null && si.RestaurantID == restaurantID)
				.ToEnumerableAsync ();

			var realItems = new List<T> ();
			foreach(var ai in storageItems){
				_logger.Debug ("Rehydrating item of type " + type + " and id " + ai.EntityID);
				var dto = ai.ToRestaurantDTO ();
				if(dto == null){
					throw new Exception ("Error turning item " + ai.EntityID + " to RestaurantDTO!");
				} else {
					var real = _entityDTOFactory.FromDataStorageObject<T> (dto);
					if(real == null){
						throw new Exception ("Error rehydrating item ID " + dto.ID);
					} else {
						realItems.Add (real);
					}
				}
			}
			return realItems;
		}
	}
}

