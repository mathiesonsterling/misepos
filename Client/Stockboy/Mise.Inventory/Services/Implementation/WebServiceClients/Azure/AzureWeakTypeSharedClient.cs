using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Base;

using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;

using Mise.Core.Entities.Accounts;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Entities.DTOs;

using Mise.Core.ValueItems;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Vendors;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Services.WebServices;
using Mise.Inventory.Services.Implementation.WebServiceClients.Exceptions;
using System.Net;
using Mise.Core.Client.Services;



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
		private readonly IDeviceConnectionService _deviceConnectionService;
		private bool _needsSynch = false;
		private DateTimeOffset? _lastTimePushed;
		public AzureWeakTypeSharedClient (ILogger logger, IJSONSerializer serializer, IMobileServiceClient client,
		IDeviceConnectionService devConnectionService)
		{
			_logger = logger;
			_client = client;
			_serial = serializer;
			_eventDTOFactory = new EventDataTransportObjectFactory (_serial);
			_entityDTOFactory = new EntityDataTransportObjectFactory (_serial);
			_deviceConnectionService = devConnectionService;

			_deviceConnectionService.ConnectionStateChanged += ConnectionStateChanged;
		}

		async void ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs args){
			if(args.CanReachMiseServer){
				if(_needsSynch){
					var res = await SynchWithServer ();

					if(res){
						_needsSynch = false;
					}
				}
			}
		}

		public Task<bool> SynchWithServer ()
		{
			return AttemptPush ();
			//TODO do we need to pull tables as well?
		}

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
			try{
				var items = await GetEntityOfTypeForRestaurant<Mise.Core.Common.Entities.Inventory.Inventory> (restaurantID);
		    return items;
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Fatal);
				throw;
			}
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

			var vendType = typeof(Vendor).ToString ();

			var query = table.Where (ai => ai.MiseEntityType == vendType);

			try{
				await AttemptPull ("allVendors", query);
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Error);
			}

			var ais = await table
				.Where (si => si.MiseEntityType == vendType)
				.ToCollectionAsync ();

			//todo figure out a better way to do this on the server
			var vendors = ais.Select(ai => ai.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<Vendor> (dto))
				.ToList();

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
				if (real == null) {
					throw new Exception ("Error rehydrating item ID " + dto.ID);
				}
				return real;
			}
		}

		#endregion



		#region IInventoryEmployeeWebService implementation

		public async Task<IEnumerable<Employee>> GetEmployeesAsync ()
		{
			var empType = typeof(Employee).ToString ();

		    var table = GetEntityTable();
			await AttemptPull ("allEmployees", null);
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
			//TODO change this to do the query on server, or at least limit it somehow
			try{
				var items = (await GetEmployeesAsync ()).ToList();
					var found = items.FirstOrDefault (e => e.PrimaryEmail != null && e.PrimaryEmail.Equals (email)
				&& e.Password != null && e.Password.Equals (password));

				if(found == null) {
					if (items.Any (e => e.PrimaryEmail != null && e.PrimaryEmail.Equals (email))) {
						throw new UserNotFoundException (email, false, true);
					}
					throw new UserNotFoundException (email);
				} 

				return found;
			} 
			catch(MobileServicePushFailedException pe){
				//check the push result to see if this is deletion of a mod that we can ignore
				foreach(var err in pe.PushResult.Errors){
					_logger.Log ("Push result error " + err.RawResult);
					if(err.Status == HttpStatusCode.NotFound){
						
					}
				}
				_logger.HandleException (pe);
				throw;
			}
			catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
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

	
		private async Task<bool> SendEventDTOs(ICollection<EventDataTransportObject> dtos){
	
			//TODO actually store the events at some point
			return true;
			var table = GetEventTable ();

			var dtoIDs = dtos.Select (dto => dto.ID).ToList();
			var existingEvents = await table.Where(ev => dtoIDs.Contains (ev.EventID)).ToEnumerableAsync ();

			//get those that exist and those that don't
			var createTasks = dtos
				.Select(dto => new AzureEventStorage (dto))
				.Select (si => table.InsertAsync (si));

			var delTasks = existingEvents.Select (ev => table.DeleteAsync (ev));
			
			try{
				//await Task.WhenAll (delTasks);
				await Task.WhenAll (createTasks);
				return true;
			} catch(Exception e){
				_logger.HandleException (e);
				//TODO prevent bad exception sends but good entity ones from blowing us up
				return true;
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
				try{
					await table.UpdateAsync (storageItem);
				} catch(Exception e){
					_logger.HandleException (e);
				}
			} else{
				try{
					await table.InsertAsync (storageItem);
				} catch(Exception e){
					_logger.HandleException (e);
				}
			}

			//TODO make await false, and only push if online!
			await AttemptPush ();


			return true;
		}

		private async Task<bool> AttemptPush(){
			try{
				await _client.SyncContext.PushAsync ();
				_lastTimePushed = DateTimeOffset.UtcNow;
				return true;
			} catch(MobileServicePushFailedException me){
				//could mean we're offline!
				_logger.HandleException (me, LogLevel.Debug);
				_needsSynch = true;
				return false;
			}
		}

		private async Task AttemptPull(string queryName, IMobileServiceTableQuery<AzureEntityStorage> query){
			try{
				var table = GetEntityTable ();
				if(query == null){
					query = table.CreateQuery ();
				}
				await table.PullAsync (queryName, query);
			} catch(WebException we){
				_logger.HandleException (we, LogLevel.Debug);
				_needsSynch = true;
			}
		}

	    private IMobileServiceSyncTable<AzureEntityStorage> GetEntityTable() 
	    {
			var table = _client.GetSyncTable<AzureEntityStorage>();
		
			return table;
	    }

		private IMobileServiceTable<AzureEventStorage> GetEventTable ()
		{
			return _client.GetTable<AzureEventStorage> ();
		}

		private string GetQueryID(string type, Guid restaurantID){
			if(type.Length > 40){
				type = type.Substring (type.Length - 40);
			}

			var hash = restaurantID.GetHashCode ();
			var res=  type + "_" + hash;
			if(res.Length > 50){
				return res.Substring (res.Length - 49);
			}

			return res;
		}

	    private async Task<IEnumerable<T>> GetEntityOfTypeForRestaurant<T>(Guid restaurantID) where T:class, IEntityBase, new()
		{
			var type = typeof(T).ToString ();

	        var table = GetEntityTable();
			//TODO get query into our pull async code
			var query = table.Where (si => si.MiseEntityType == type && si.RestaurantID != null && si.RestaurantID == restaurantID);
			var queryID = GetQueryID (type, restaurantID);

			try{
				await AttemptPull (queryID, query);
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Error);
			}

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
					if (real == null) {
						throw new Exception ("Error rehydrating item ID " + dto.ID);
					}
					realItems.Add (real);
				}
			}
			return realItems;
		}

	}
}

