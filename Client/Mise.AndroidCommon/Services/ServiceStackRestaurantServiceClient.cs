using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Mise.Core.Services.WebServices;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People.Events;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Restaurant.Events;

using Mise.Core.Common.Events.DTOs;
using ServiceStack;
using Mise.Core.Common.Entities;
using Mise.Core.Services;
using Mise.Core.Entities.Restaurant;

namespace Mise.AndroidCommon.Services
{
	#region Temp request holding part
	[Route("/api/{RestaurantFriendlyName*}", "GET")]

	public class RestaurantRequest : IReturn<Restaurant>
	{
		[ApiMember(IsRequired = true)]
		public string RestaurantFriendlyName { get; set; }

		/// <summary>
		/// If set, the terminal that is requesting this
		/// </summary>
		public string TerminalName { get; set; }
	}

	[Route("/api/{RestaurantFriendlyName}/Checks/{CheckID*}", "GET")]
	public class CheckRequest : IReturn<IEnumerable<ICheck>>
	{
		[ApiMember(IsRequired = true)]
		public string RestaurantFriendlyName { get; set; }

		public Guid? CheckID { get; set; }

		public CheckPaymentStatus? PaymentStatus { get; set; }
	}

	[Route("/api/{RestaurantFriendlyName}/Checks/", "POST")]
	public class CheckEventRequest : IReturnVoid
	{
		[ApiMember(IsRequired = true)]
		public string RestaurantFriendlyName { get; set; } 
		[ApiMember(IsRequired = true)]
		public EventDataTransportObject[] Events { get; set; } 
	}

	[Route("/api/{RestaurantFriendlyName}/Menu/{MenuName*}", "GET")]
	public class MenuRequest : IReturn<IEnumerable<Menu>>
	{
		[ApiMember(IsRequired = true)]
		public string RestaurantFriendlyName { get; set; }

		public string MenuName { get; set; }
	}

	[Route("/api/{RestaurantFriendlyName}/Employees/{EmployeeID*}", "GET")]
	public class EmployeeRequest : IReturn<IEnumerable<IEmployee>>
	{
		[ApiMember(IsRequired = true)]
		public string RestaurantFriendlyName { get; set; }

		/// <summary>
		/// If set, we only want the employee with the given ID
		/// </summary>
		public Guid? EmployeeID { get; set; }

		/// <summary>
		/// If set, we only want employees with matching status
		/// </summary>
		public bool? ClockedIn { get; set; }
	}

	[Route("/api/{RestaurantFriendlyName}/Employees/", "POST")]
	public class EmployeeEventsRequest : IReturnVoid
	{
		[ApiMember(IsRequired = true)]
		public string RestaurantFriendlyName { get; set; }
		[ApiMember(IsRequired = true)]
		public EventDataTransportObject[] Events { get; set; } 
	}
	#endregion

	public class ServiceStackRestaurantServiceClient : IRestaurantTerminalService
	{
		private JsonServiceClient _client;
		private EventDataTransportObjectFactory _dtoFactory;
		private string _restaurantFriendlyName;
		ILogger _logger;
		public ServiceStackRestaurantServiceClient (string apiBaseAddress, IJSONSerializer serializer, string restaurantFriendlyName, ILogger logger)
		{
			//TODO we'll likely need to move this to a more neutral area at some point
			AndroidPclExportClient.Configure();
			_client = new JsonServiceClient (apiBaseAddress);
			_dtoFactory = new EventDataTransportObjectFactory (serializer);

			_restaurantFriendlyName = restaurantFriendlyName;
			_logger = logger;
		}

		public ServiceStackRestaurantServiceClient(Uri apiBase, IJSONSerializer serializer, IRestaurant restaurant, ILogger logger) : this(apiBase.AbsoluteUri, serializer, restaurant.FriendlyID, logger){

		}
			

		#region IRestaurantTerminalService implementation

		public Task<Tuple<IRestaurant, IMiseTerminalDevice>> RegisterClientAsync (string deviceName)
		{
			//get restaurant first
			var request = new RestaurantRequest { RestaurantFriendlyName = _restaurantFriendlyName, TerminalName=deviceName };
			return  _client.GetAsync (request)
				.ContinueWith(task => {
					//get terminal that matches
					var rest = task.Result as IRestaurant;
					var terminal = rest.GetTerminals ().FirstOrDefault (t => t.IsMe);

					return new Tuple<IRestaurant, IMiseTerminalDevice>(rest, terminal);
				});
		}
		public Task<IEnumerable<IEmployee>> GetEmployeesAsync ()
		{
			var req = new EmployeeRequest {
				RestaurantFriendlyName = _restaurantFriendlyName
			};
			//return _client.GetAsync (req);
			//var res = _client.Get (req);

			return Task.Factory.StartNew (() => _client.Get(req));
		}
		public Task<IEnumerable<ICheck>> GetChecksAsync ()
		{
			var req = new CheckRequest { RestaurantFriendlyName = _restaurantFriendlyName };
			return Task.Factory.StartNew (() => {
				var checks = _client.Get (req);
				return checks;
			});
			//return _client.GetAsync (req);
		}
		public Task<IEnumerable<Menu>> GetMenusAsync ()
		{
			//TODO get the menu item rules as well
			return GetMenus ();
		}

		private async Task<IEnumerable<Menu>> GetMenus(){
			var results = await _client.GetAsync(new MenuRequest {RestaurantFriendlyName = _restaurantFriendlyName});
			return results.ToList<Menu>();
		}

		public Task<bool> SendEventsAsync (IEnumerable<ICheckEvent> events)
		{
			//TODO figure out how to use their async methods instead!
			return Task.Factory.StartNew (() => {
				try {
					var dtos = events.Select (dto => _dtoFactory.ToDataTransportObject (dto)).ToArray();
					var req = new CheckEventRequest {
						RestaurantFriendlyName = _restaurantFriendlyName,
						Events = dtos,
					};
					_client.Post (req);
					return true;
				} catch (Exception e) {
					_logger.HandleException (e);
					return false;
				}
			});
		}

		public Task<bool> SendEventsAsync (IEnumerable<IEmployeeEvent> empEvents)
		{
			//TODO figure out how to use their async methods instead!
			return Task.Factory.StartNew (() => {
				try{
					var dtos = empEvents.Select (dto => _dtoFactory.ToDataTransportObject (dto)).ToArray ();
					var req = new EmployeeEventsRequest {
						RestaurantFriendlyName = _restaurantFriendlyName,
						Events = dtos
					};
					_client.Post (req);
					return true;
				} catch(Exception e){
					_logger.HandleException (e);
					return false;
				}
			});
			//return _client.PostAsync (req);
		}

		public Task<bool> SendEventsAsync(IEnumerable<IRestaurantEvent> restEvents){
			throw new NotImplementedException ();
		}

		public Task SendOrderItemsToDestination (OrderDestination destination, OrderItem orderItem)
		{
			throw new NotImplementedException ();
		}
		public Task NotifyDestinationOfVoid (OrderDestination destination, OrderItem orderItem)
		{
			throw new NotImplementedException ();
		}
		#endregion

		public Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant (Guid restaurantID)
		{
			throw new NotImplementedException ();
		}

		public Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password)
		{
			throw new NotImplementedException ();
		}
	}
}

