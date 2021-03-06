﻿using System;
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
using Mise.Core.Common.Entities.People;
using Newtonsoft.Json.Serialization;
using Mise.Core.Entities.Inventory;

using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
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

        public static MobileServiceSQLiteStore DefineTables(MobileServiceSQLiteStore store)
        {
            store.DefineTable<AzureEntityStorage>();
            store.DefineTable<AzureEventStorage>();
            return store;
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
			try{
				return AttemptPush ();
			} catch(Exception e){
				_logger.HandleException (e);
				return Task.FromResult (false);
			}
			//TODO do we need to pull tables as well?
		}

        public Task SetRestaurantId(Guid restaurantId)
        {
            return Task.FromResult(true);
        }

		public async Task<bool> SendEventsAsync (RestaurantAccount updatedEntity, IEnumerable<IAccountEvent> events)
		{

			var entRes = await StoreEntity (updatedEntity);

            var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();
			var evRes = await SendEventDTOs (dtos);

			return entRes;
		}

		public async Task<bool> SendEventsAsync (ApplicationInvitation updatedEntity, IEnumerable<Mise.Core.Entities.Restaurant.Events.IApplicationInvitationEvent> events)
		{
			var entRes = await StoreEntity(updatedEntity);

            var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();
			var evRes = await SendEventDTOs (dtos);

			return entRes;
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
			var entRes = await StoreEntity (updatedEntity);

            var dtos = events.Select (ev => _eventDTOFactory.ToDataTransportObject (ev)).ToList ();
			var evRes = await SendEventDTOs (dtos);

			return entRes;
		}

		public async Task<bool> SendEventsAsync (Core.Common.Entities.Inventory.Inventory updatedEntity, IEnumerable<Mise.Core.Entities.Inventory.Events.IInventoryEvent> events)
		{
            if (updatedEntity.IsEmpty)
            {
                _logger.Error("Empty inventory saved, id " + updatedEntity.Id);
            }

			var entRes = await StoreEntity (updatedEntity);

			return entRes;
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
            var query = table.Where(si => si.MiseEntityType == type.ToString() && si.EntityJSON.Contains(email.Value));

            await AttemptPull("invitationsFor" + email.Value, query);
			var storageItems = await query.ToEnumerableAsync ();

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
            var query = table.Where(ai => ai.MiseEntityType == vendType.ToString());
            await AttemptPull("vendorsFor" + restaurantID, query);
			var ais = await query.ToEnumerableAsync ();

			//todo figure out a better way to do this on the server
			//todo also remove to List when debugging is done!
			var vendors = ais.Select(ai => ai.ToRestaurantDTO ())
				.Select (dto => _entityDTOFactory.FromDataStorageObject<Vendor> (dto)).ToList();
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

            var query = table.Where(ai => ai.MiseEntityType == restType);
            await AttemptPull("allRests", query);
			var azureItems = await query.ToEnumerableAsync ();

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
            if (restaurantID == Guid.Empty)
            {
                return null;
            }
			var type = typeof(Restaurant).ToString ();

			var table = GetEntityTable();

            var query = table
                .Where(si => si.MiseEntityType == type && si.EntityID == restaurantID);

            await AttemptPull("rest" + restaurantID, query);

			var storageItems = await query.ToEnumerableAsync ();

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
					throw new Exception ("Error rehydrating item ID " + dto.Id);
				}
				return real;
			}
		}

		#endregion



		#region IInventoryEmployeeWebService implementation

		public async Task<bool> IsEmailRegistered (EmailAddress email)
		{
			var empType = typeof(Employee).ToString ();
			var table = GetEntityTable ();
			try{
				await AttemptPull ("allEmployees", null);
			} 	catch(MobileServicePushFailedException pe){
				//check the push result to see if this is deletion of a mod that we can ignore
				_logger.HandleException (pe);
				throw;
			}

			var ais = await table.Where (ai => ai.MiseEntityType == empType && ai.EntityJSON.Contains(email.Value)).ToEnumerableAsync ();

			foreach (var ai in ais) {
				var desered = _entityDTOFactory.FromDataStorageObject<Employee> (ai.ToRestaurantDTO());
				if (desered != null) {
					if ((desered.PrimaryEmail != null && desered.PrimaryEmail.Equals (email))
					   || (desered.GetEmailAddresses ().Any (em => em.Equals (email)))) {
						return true;
					}
				}
			}

			return false;

		}

		public async Task<IEnumerable<Employee>> GetEmployeesAsync ()
		{
			var empType = typeof(Employee).ToString ();
		    var table = GetEntityTable();
			try{
				await AttemptPull ("allEmployees", null);
			} 	catch(MobileServicePushFailedException pe){
				//check the push result to see if this is deletion of a mod that we can ignore
				_logger.HandleException (pe);
				throw;
			}

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
			try{
                var emailString = email.Value;
                var empType = typeof(Employee).ToString ();
                var table = GetEntityTable();

                var query = table.Where (ai => ai.MiseEntityType == empType && ai.EntityJSON.Contains(emailString));
                await AttemptPull ("emp::"+emailString, query);

                var ais = await query.ToEnumerableAsync ();
                var items = ais.Select (ai => ai.ToRestaurantDTO ())
                    .Select (dto => _entityDTOFactory.FromDataStorageObject<Employee> (dto));
                
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
			catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}
		#endregion

        public async Task<IAccount> GetAccountById(Guid id){
            var restAcctType = typeof(RestaurantAccount).ToString ();
            var table = GetEntityTable();

            var query = table.Where (ai => ai.MiseEntityType == restAcctType && ai.EntityID == id);
            await AttemptPull ("acct::"+id.ToString(), query);

            var ais = await query.ToEnumerableAsync ();
            var items = ais.Select (ai => ai.ToRestaurantDTO ())
                .Select (dto => _entityDTOFactory.FromDataStorageObject<RestaurantAccount> (dto));

            var found = items.FirstOrDefault ();

            return found;
        }

        public async Task<IAccount> GetAccountFromEmail(EmailAddress email){
            var emailString = email.Value;
            var restAcctType = typeof(RestaurantAccount).ToString ();
            var table = GetEntityTable();

            var query = table.Where (ai => ai.MiseEntityType == restAcctType && ai.EntityJSON.Contains(emailString));
            await AttemptPull ("acct::"+emailString, query);

            var ais = await query.ToEnumerableAsync ();
            var items = ais.Select (ai => ai.ToRestaurantDTO ())
                .Select (dto => _entityDTOFactory.FromDataStorageObject<RestaurantAccount> (dto));

            var found = items.FirstOrDefault (a => a.PrimaryEmail != null && a.PrimaryEmail.Equals (email));

            return found;
        }

        public async Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersForRestaurant(Guid restaurantId){
            var items = await GetEntityOfTypeForRestaurant<PurchaseOrder>(restaurantId);
            return items;
        }

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
            return true;


			var table = GetEventTable ();

			var dtoIDs = dtos.Select (dto => dto.Id).ToList();
			var existingEvents = await table.Where(ev => dtoIDs.Contains (ev.EventID)).ToEnumerableAsync ();

			//get those that exist and those that don't
			var createTasks = dtos
				.Select(dto => new AzureEventStorage (dto))
				.Select (si => table.InsertAsync (si));

			//var delTasks = existingEvents.Select (ev => table.DeleteAsync (ev));
			
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
			var existing = (await table.Where(ai => ai.EntityID == dto.Id).ToEnumerableAsync ()).FirstOrDefault ();
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
				return true;
			} catch(MobileServicePushFailedException me){
				//could mean we're offline!
				_logger.HandleException (me, LogLevel.Debug);
				_needsSynch = true;
				return false;
			}

			//https://codemilltech.com/why-cant-we-be-friends-conflict-resolution-in-azure-mobile-services/
		}

		private async Task AttemptPull(string queryName, IMobileServiceTableQuery<AzureEntityStorage> query){
            var triedAgain = false;
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
            catch(MobileServiceInvalidOperationException ue){
                _logger.HandleException (ue);
                if (triedAgain)
                {
                    throw;
                }
                triedAgain = true;
                _entityTable = null;
                var table = GetEntityTable();
                if (query == null)
                {
                    query = table.CreateQuery();
                }
                await table.PullAsync(queryName, query);
            }
		}

        private IMobileServiceSyncTable<AzureEntityStorage> _entityTable;
	    private IMobileServiceSyncTable<AzureEntityStorage> GetEntityTable() 
	    {
            if (_entityTable == null)
            {
                _entityTable = _client.GetSyncTable<AzureEntityStorage>();
            }
		
			return _entityTable;
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
						throw new Exception ("Error rehydrating item ID " + dto.Id);
					}
					realItems.Add (real);
				}
			}
			return realItems;
		}

	}
}

