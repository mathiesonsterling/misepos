namespace Mise.Inventory.Services.Implementation.WebServiceClients
{ /*using ServiceStack;
namespace Mise.Inventory
{
	public class ServiceStackWebServiceClient : IInventoryEmployeeWebService, 
	IInventoryRestaurantWebService, IVendorWebService,
	IPARWebService, IInventoryWebService,
	IReceivingOrderWebService, IPurchaseOrderWebService,
	IDisposable
	{
		readonly Uri _serverLocation;
		readonly string _deviceID;
		readonly IServiceClient _serviceClient;
		readonly IJSONSerializer _serializer;
		readonly EventDataTransportObjectFactory _dtoFactory;
		readonly ILogger _logger;
		public ServiceStackWebServiceClient(Uri serverLocation, string deviceID, IJSONSerializer serializer, ILogger logger){
			_logger = logger;
			_serverLocation = serverLocation;
			_deviceID = deviceID;
			_serviceClient = new JsonServiceClient (_serverLocation.ToString ());
			_serializer = serializer;
			_dtoFactory = new EventDataTransportObjectFactory (serializer);
		}

		#region IEventStoreWebService implementation
		async Task<bool> SendDTOSAsync(IEnumerable<EventDataTransportObject> dtos){
			var restID = dtos.First ().RestaurantID;
			var req = new EventSubmissionRequest {
				Events = dtos,
				DeviceID = _deviceID,
				RestaurantID = restID
			};

			var res = await _serviceClient.PostAsync(req);
			var success =  res.Result;
			if (success == false) {
				_logger.Log (res.ErrorMessage);
			}
			_logger.Debug (res.NumEventsProcessed + " events processed");
			return success;
		}

		public Task<bool> SendEventsAsync (IEnumerable<IEmployeeEvent> events)
		{
			var dtos = events.Select (e => _dtoFactory.ToDataTransportObject (e));

			var debug = dtos.ToList ();
			return SendDTOSAsync(dtos);
		}

		public Task<bool> SendEventsAsync(IEnumerable<IRestaurantEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)));
		}

		public Task<bool> SendEventsAsync(IEnumerable<IVendorEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)));
		}

		public Task<bool> SendEventsAsync(IEnumerable<IPAREvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)));
		}

		public Task<bool> SendEventsAsync(IEnumerable<IInventoryEvent> events)
		{
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)));
		}

		public Task<bool> SendEventsAsync(IEnumerable<IReceivingOrderEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)));
		}

		public Task<bool> SendEventsAsync(IEnumerable<IPurchaseOrderEvent> events){
			return SendDTOSAsync(events.Select(e => _dtoFactory.ToDataTransportObject(e)));
		}
		#endregion

		#region IInventoryEmployeeWebService implementation
		public async Task<IEnumerable<IEmployee>> GetEmployeesAsync ()
		{
			var empRequest = new EmployeeRequest {
			};
			var res = await _serviceClient.GetAsync (empRequest);
			return res.Results.Cast<IEmployee> ();
		}

		public async Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			var empRequest = new EmployeeRequest {
				RestaurantID = restaurantID
			};
			var res = await _serviceClient.GetAsync (empRequest);
			return res.Results.Cast<IEmployee> ();
		}

		public async Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			var req = new EmployeeRequest {
				Email = email.Value,
				PasswordHash = password.HashValue
			};
			var res = await _serviceClient.GetAsync (req);
			return res.Results.FirstOrDefault ();
		}

		#endregion

		public async Task<IEnumerable<IRestaurant>> GetRestaurants (Location deviceLocation)
		{
			var req = new RestaurantRequest {
				Latitude = deviceLocation.Latitude,
				Longitude = deviceLocation.Longitude
			};
			var res = await _serviceClient.GetAsync (req);
			return res.Results;
		}

		public async Task<IRestaurant> GetRestaurant (Guid restaurantID)
		{
			var req = new RestaurantRequest {
				RestaurantID = restaurantID
			};
			var res = await _serviceClient.GetAsync (req);
			return res.Results.FirstOrDefault();
		}

		public async Task<IEnumerable<IVendor>> GetVendorsWithinSearchRadius (Location currentLocation, Distance radius)
		{
			var req = new VendorRequest {
				RestaurantID = null,
				RadiusInKm = radius.Kilometers,
				Longitude = currentLocation.Longitude,
				Latitude = currentLocation.Latitude
			};
			var res = await _serviceClient.GetAsync (req);
			return res.Results;
		}

		public Task<IEnumerable<IVendor>> GetVendorsBySearchString (string searchString)
		{
			throw new NotImplementedException ();
		}

		public async Task<IEnumerable<IVendor>> GetVendorsAssociatedWithRestaurant (Guid restaurantID)
		{
			var req = new VendorRequest {
				RestaurantID = restaurantID
			};
			var res = await _serviceClient.GetAsync(req);
			return res.Results;
		}

		public async Task<IPAR> GetCurrentPAR (Guid restaurantID)
		{
			var items = await GetPARsForRestaurant (restaurantID);
			return items.FirstOrDefault (p => p.IsCurrent);
		}

		public async Task<IEnumerable<IPAR>> GetPARsForRestaurant (Guid restaurantID)
		{
			var req = new PARRequest {
				RestaurantID = restaurantID
			};

			var res = await _serviceClient.GetAsync (req);
			return res.Results;
		}

		public async Task<IEnumerable<IInventory>> GetInventoriesForRestaurant (Guid restaurantID)
		{
			var req = new InventoryRequest {
				RestaurantID = restaurantID
			};

			var res = await _serviceClient.GetAsync (req);
			return res.Results.Cast<IInventory>();
		}

		public async Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant (Guid restaurantID)
		{
			var req = new ReceivingOrderRequest {
				RestaurantID = restaurantID
			};

			var res = await _serviceClient.GetAsync(req);
			return res.Results;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			if (_serviceClient != null) {
				_serviceClient.Dispose ();
			}
		}

		#endregion
	}
}*/
}