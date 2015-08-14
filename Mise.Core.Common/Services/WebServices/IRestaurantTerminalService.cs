using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.WebServices
{

    /// <summary>
    /// Webservice for the restaurant, used to power terminals
    /// </summary>
    public interface IRestaurantTerminalService : IEventStoreWebService<RestaurantCheck, ICheckEvent>, IInventoryEmployeeWebService, IInventoryRestaurantWebService
    {
        /// <summary>
        /// Register the client with the service, and give us the restaurant client with it
        /// </summary>
        /// <returns></returns>
		Task<Tuple<Restaurant, IMiseTerminalDevice>> RegisterClientAsync(string deviceName);


        /// <summary>
        /// Get all check this terminal should be dealing with
        /// </summary>
        /// <returns></returns>
		Task<IEnumerable<RestaurantCheck>> GetChecksAsync();

        /// <summary>
        /// Get the current menu for our repository
        /// </summary>
        /// <returns></returns>
		Task<IEnumerable<Menu>> GetMenusAsync();
        

		Task SendOrderItemsToDestination(OrderDestination destination, OrderItem orderItem);

		Task NotifyDestinationOfVoid(OrderDestination destination, OrderItem orderItem);
    }
}
