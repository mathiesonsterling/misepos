using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ModernHttpClient;
namespace Mise.Inventory.Services.Implementation
{
	public class HttpWebServiceClient : IInventoryApplicationWebService,
	IDisposable
	{
		readonly HttpClient _client;
		readonly Uri _serverLocation;
		readonly string _deviceID;
		readonly IJSONSerializer _serializer;
		readonly EventDataTransportObjectFactory _dtoFactory;
		readonly ILogger _logger;


		public HttpWebServiceClient(Uri serverLocation, string deviceID, IJSONSerializer serializer, ILogger logger){
			_logger = logger;
			_serverLocation = serverLocation;
			_deviceID = deviceID;
			_client = new HttpClient(new NativeMessageHandler());
			_serializer = serializer;
			_dtoFactory = new EventDataTransportObjectFactory (serializer);
		}
			
		#region IInventoryEmployeeWebService implementation
		public async Task<IEnumerable<IEmployee>> GetEmployeesAsync ()
		{
			var empsResponse = await GetData<EmployeeResponse> ("employees");
			return empsResponse != null ? empsResponse.Results : new List<IEmployee> ().AsEnumerable ();

		}

		public async Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			var empsResponse = await GetData<EmployeeResponse> ("restaurants/" + restaurantID + "/employees");
			if (empsResponse != null){
				return empsResponse.Results;
			}

			return new List<IEmployee> ();
		}

		//    [Route("/employees/{Email}/{PasswordHash}")]
		public async Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			var empsResponse = await GetData<EmployeeResponse> ("/employees/" + email.Value + "/" + password.HashValue);
			if (empsResponse != null){
				return empsResponse.Results.FirstOrDefault();
			}

			return null;
		}

		#endregion

		public async Task<IEnumerable<IVendor>> GetVendorsWithinSearchRadius (Location currentLocation, Distance radius)
		{
			var res = await GetData<VendorResponse> ("vendors/" + currentLocation.Longitude + "/" + currentLocation.Latitude + "/" + radius.Kilometers);
			return res.Results;
		}

		public async Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant (Guid restaurantID)
		{
			var res = await GetData<VendorResponse> ("restaurants/" + restaurantID + "/vendors");
			return res.Results;
		}
			
		public async Task<IEnumerable<IRestaurant>> GetRestaurants (Location deviceLocation)
		{
			var res = await GetData<RestaurantResponse>("restaurants");
			return res.Results;
		}

		public async Task<IRestaurant> GetRestaurant (Guid restaurantID)
		{
			var res = await GetData<RestaurantResponse>("restaurants/" + restaurantID);
			return res.Results.FirstOrDefault();
		}

		public async Task<IPAR> GetCurrentPAR (Guid restaurantID)
		{
			var pars = await GetPARsForRestaurant (restaurantID);
			return pars.FirstOrDefault (p => p.IsCurrent);
		}

		//    [Route("/restaurants/{RestaurantID}/pars/")]
		//[Route("/restaurants/{RestaurantID}/pars/{PARID}")]
		public async Task<IEnumerable<IPAR>> GetPARsForRestaurant (Guid restaurantID)
		{
			var res = await GetData<PARResponse> ("restaurants/" + restaurantID + "/pars");
			return res.Results;
		}

		//    [Route("/restaurants/{RestaurantID}/inventories")]
		//[Route("/restaurants/{RestaurantID}/inventories/{InventoryID}")]
		public async Task<IEnumerable<IInventory>> GetInventoriesForRestaurant (Guid restaurantID)
		{
			var res = await GetData<InventoryResponse>("restaurants/"+restaurantID+"/inventories");
			return res.Results.Cast<IInventory> ();
		}
			
		public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant (Guid restaurantID)
		{
			var res = await GetData<ReceivingOrderResponse>("restaurants/"+restaurantID+"/receivingorders");
			return res.Results;
		}


        public async Task<IEnumerable<IApplicationInvitation>> GetInvitationsForRestaurant(Guid restaurantID)
        {
            var res = await GetData<ApplicationInvitationResponse>("restaurants/" + restaurantID + "/invitations");
            return res.Results;
        }

        public async Task<IEnumerable<IApplicationInvitation>> GetInvitationsForEmail(EmailAddress email)
        {
            var res = await GetData<ApplicationInvitationResponse>("invitations/" + email.Value);
            return res.Results;
        }

		#region IEventStoreWebService implementation

		async Task<bool> SendDTOSAsync(ICollection<EventDataTransportObject> dtos){
			var restID = dtos.First ().RestaurantID;
			var req = new EventSubmission{
				Events = dtos,
				DeviceID = _deviceID,
				RestaurantID = restID
			};
					
			await PostData("events", req);

			_logger.Debug ("events processed");
			return true;
		}

		public Task<bool> SendEventsAsync (IEmployee emp, IEnumerable<IEmployeeEvent> events)
		{
			var dtos = events.Select (e => _dtoFactory.ToDataTransportObject (e));

			var debug = dtos.ToList ();
			return SendDTOSAsync(debug);
		}

		public Task<bool> SendEventsAsync(IRestaurant rest, IEnumerable<IRestaurantEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
		}

		public Task<bool> SendEventsAsync(IVendor vend, IEnumerable<IVendorEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
		}

		public Task<bool> SendEventsAsync(IPAR par, IEnumerable<IPAREvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
		}

		public Task<bool> SendEventsAsync(IInventory inv, IEnumerable<IInventoryEvent> events)
		{
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
		}

		public Task<bool> SendEventsAsync(IReceivingOrder ro, IEnumerable<IReceivingOrderEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
		}

		public Task<bool> SendEventsAsync(IPurchaseOrder po, IEnumerable<IPurchaseOrderEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
		}

        public Task<bool> SendEventsAsync(IApplicationInvitation invite, IEnumerable<IApplicationInvitationEvent> events)
        {
            return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
        }

		public Task<bool> SendEventsAsync(IAccount acc, IEnumerable<IAccountEvent> events)
        {
            return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)).ToList());
        }

	    public Task<bool> ResendEvents(ICollection<IEntityEventBase> events)
	    {
	        var needTransform = events.Where(ev => ev is EventDataTransportObject == false).ToList();
	        if (needTransform.Any() == false)
	        {
	            var sendPlain = events.Cast<EventDataTransportObject>();

                //do this in chunks
	            return SendDTOSAsync(sendPlain.ToList());
	        }
	        //we'll need to split these up
	        throw new InvalidOperationException("Currently only can send already transformed events!");
	    }

	    #endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			if (_client != null) {
				_client.Dispose ();
			}
		}

		#endregion


		async Task PostData<T>(string urlSub, T data){
			try{
				var dataString = _serializer.Serialize (data);
				var url = _serverLocation.AbsoluteUri  + urlSub;
				var res = await _client.PostAsync (url, new StringContent (dataString, System.Text.Encoding.UTF8, 
					"application/json"));
				if(res.IsSuccessStatusCode == false){
					//is it due to the server not being ready?
					throw new SendEventsException(res.ReasonPhrase);
				}
			}catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		async Task<T> GetData<T>(string urlSub) where T:class
		{
			string url;
			var serverUrl = _serverLocation.AbsoluteUri;
			if (serverUrl.EndsWith ("/", StringComparison.Ordinal) && urlSub.StartsWith ("/", StringComparison.Ordinal)) {
				url = _serverLocation.AbsoluteUri + urlSub.Substring(1) + "?format=json";
			} else {
				url = _serverLocation.AbsoluteUri + urlSub + "?format=json";
			}

			try{
				var res = await _client.GetStringAsync (url);
				return _serializer.Deserialize<T> (res);
			} catch(Exception e){
				_logger.HandleException (e);
				//TODO if this is because the server isn't up, throw a ServerNotReadyException wrapping this
				throw;
			}
		}


	}
}

