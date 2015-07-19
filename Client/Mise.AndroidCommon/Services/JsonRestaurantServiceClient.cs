using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using Mise.Core.Services.WebServices;

using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;

using Mise.Core.Common.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.People.Events;
using Mise.Core.Services;

using Mise.Core.Common.Events.DTOs;
namespace Mise.AndroidCommon.Services
{			
	public class JsonRestaurantServiceClient : IRestaurantTerminalService
	{

		Uri _restaurantServerURL;
		string _restaurantFriendlyID;
		string _deviceID;

		IJSONSerializer _serializer;
		WebClient _webClient;
		ILogger _logger;
		EventDataTransportObjectFactory _dtoFactory;
		int _numTimesRegistrationAttempted = 0;
		public JsonRestaurantServiceClient(IJSONSerializer serializer, ILogger logger, string machineID, IRestaurant restaurant){
			_webClient = new WebClient();
			_webClient.Headers[HttpRequestHeader.ContentType] = "application/json";

			_deviceID = machineID;
			_serializer = serializer;
			_logger = logger;
			_dtoFactory = new EventDataTransportObjectFactory (_serializer);
			_restaurantServerURL = restaurant.RestaurantServerLocation;
			_restaurantFriendlyID = restaurant.FriendlyID;
			_numTimesRegistrationAttempted = 0;
		}

		#region IRestaurantTerminalService implementation

		public Task<Tuple<IRestaurant,IMiseTerminalDevice>> RegisterClientAsync (string deviceName)
		{
			return Task.Factory.StartNew (() => {
				_numTimesRegistrationAttempted++;

				//get the restaurant
				var restUrl = _restaurantServerURL + "api/" + _restaurantFriendlyID + "?format=json";

				/*var downloadTask = _webClient.DownloadStringTaskAsync (new Uri(restUrl));
				var json = downloadTask.Result;*/
				var json = _webClient.DownloadString (restUrl);
				var restaurant = _serializer.Deserialize<Restaurant> (json);

				//retrieve our terminal
				var term = restaurant.Terminals.FirstOrDefault (t => t.FriendlyID == deviceName);

				//TODO push up our new terminal here if we don't exist already!
				if (term == null) {
					term = new MiseTerminalDevice {
						ID = Guid.NewGuid (),
						FriendlyID = deviceName,
						CreatedDate = DateTime.UtcNow,
						LastUpdatedDate = DateTime.UtcNow,
						MachineID = _deviceID,
					};
					//api/restaurants/{restaurantName}/terminals
					var url = _restaurantServerURL + "api/restaurants/" + _restaurantFriendlyID + "/terminals";
					var termJSON = _serializer.Serialize (term);
					var putTask = PutAsync (url, termJSON);

					var result = putTask.Result;
					if (result == false) {
						if (_numTimesRegistrationAttempted < 5) {
							Thread.Sleep (100);
							var task = RegisterClientAsync (deviceName);
							task.Wait ();
						} else {
							throw new Exception ("Unable to register terminal with restuarant service!");
						}
					}

					//we registered, so we have to repoll to get our correct items
					var downloadTask = _webClient.DownloadStringTaskAsync (new Uri (restUrl));
					json = downloadTask.Result;
					restaurant = _serializer.Deserialize<Restaurant> (json);

					//retrieve our terminal
					term = restaurant.Terminals.FirstOrDefault (t => t.MachineID == _deviceID);
				}

				return new Tuple<IRestaurant, IMiseTerminalDevice>(restaurant, term);
			});
		}

		/// <summary>
		/// Helper function for our many methods which get a list from a JSON address
		/// </summary>
		/// <returns>The collection.</returns>
		/// <param name="urlAppend">URL append.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private IEnumerable<T> GetCollection<T>(string urlAppend){
			var url = _restaurantServerURL + "api/" + urlAppend;
			var uri = new Uri (url);
			var json = _webClient.DownloadString (uri);
			var items = _serializer.Deserialize<List<T>> (json);

			return items;
		}

		async Task<IEnumerable<T>> GetCollectionAsync<T>(string urlAppend){
			var url = _restaurantServerURL + "api/" + urlAppend;
			var uri = new Uri (url);
			var json = await _webClient.DownloadStringTaskAsync (uri);
			var items = _serializer.Deserialize<List<T>> (json);

			return items;
		}

		public IEnumerable<IEmployee> GetEmployees ()
		{
			//api/employees/{restaurantName}
			return GetCollection<Employee> ("employees/" + _restaurantFriendlyID);
		}

		public async Task<IEnumerable<IEmployee>> GetEmployeesAsync ()
		{
			return await GetCollectionAsync<Employee> ("employees/" + _restaurantFriendlyID);
		}
			

		public async Task<IEnumerable<ICheck>> GetChecksAsync ()
		{
			return await GetCollectionAsync<RestaurantCheck> ("restaurants/" + _restaurantFriendlyID + "/checks");
		}
			

		public Menu GetCurrentMenu ()
		{
			//api/menus/{restaurantName}/current
			var url = _restaurantServerURL + "api/menus/" + _restaurantFriendlyID + "/current";
			var uri = new Uri (url);
			var json = _webClient.DownloadString (uri);
			var menu = _serializer.Deserialize<Menu> (json);
			return menu;
		}

		public Task<IEnumerable<Menu>> GetMenusAsync ()
		{
			var url = _restaurantServerURL + "api/" + _restaurantFriendlyID + "/menu?format=json";
			var uri = new Uri (url);
			return Task.Factory.StartNew (() => {
				//TODO figure out why async doesn't work here
				var json = _webClient.DownloadString (url);
				var array = _serializer.Deserialize<Menu[]> (json);
				return array.AsEnumerable<Menu> ();
			});
			/*
			var json = await _webClient.DownloadStringTaskAsync (uri);
			var menu = _serializer.Deserialize<Menu[]> (json);
			return menu;*/
		}

		bool Put(string url, string json){
			var uri = new Uri (url);
			try{			
				var res =  _webClient.UploadString (uri, "PUT", json);
				return  true;
			} catch(WebException we){
				var response = we.Response as HttpWebResponse;
				if (response != null) {
					return false;
				}
			}  catch(Exception e){
				_logger.HandleException (e);
				return false;
			}

			_logger.Log ("No exception thrown");
			return false;
		}

		async Task<bool> PutAsync(string url, string json){
			var uri = new Uri (url);
			try{			
				await _webClient.UploadStringTaskAsync (uri, "PUT", json);
				return true;
			} catch(WebException we){
				var response = we.Response as HttpWebResponse;
				if (response != null) {
					return false;
				}
			} catch(Exception e){
				return false;
			}

			_logger.Log ("No exception thrown");
			return false;		}
			

		public Task<bool> SendEventsAsync (IEnumerable<ICheckEvent> events)
		{
			var dtos = events.Select (e => _dtoFactory.ToDataTransportObject (e));
			var url = _restaurantServerURL + "\"/api/" + _restaurantFriendlyID + "/Checks/";
			var serialized = _serializer.Serialize (dtos);

			return PutAsync (url, serialized);
		}
			

		public Task<bool> SendEventsAsync (IEnumerable<IEmployeeEvent> empEvents)
		{
			throw new NotImplementedException ();
		}

		public Task SendOrderItemsToDestination (OrderDestination destination, OrderItem orderItem)
		{
			return Task.Factory.StartNew (() => {});
		}

		public Task<bool> SendEventsAsync(IEnumerable<IRestaurantEvent> restEvents){
			throw new NotImplementedException ();
		}

		public Task NotifyDestinationOfVoid (OrderDestination destination, OrderItem orderItem)
		{
			return Task.Factory.StartNew (() =>{
				throw new NotImplementedException ();
			});
		}





		public Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

		public Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			throw new NotImplementedException ();
		}
		#endregion


	}
}

