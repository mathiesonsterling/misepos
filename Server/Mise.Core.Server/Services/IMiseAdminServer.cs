
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Restaurant;
using System.Collections.Generic;
using Mise.Core.Entities.Check;
using System;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities;


namespace Mise.Core.Server.Services
{
	public interface IMiseAdminServer
	{
		bool SendEvents(Guid restaurantID,IEnumerable<IEntityEventBase> events);

        /// <summary>
        /// If a new terminal has registerd, but not yet on the website, we push up via this method
        /// </summary>
        /// <param name="restaurantID"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
	    bool AddNewTerminal(Guid restaurantID, IMiseTerminalDevice terminal);

		IEnumerable<ICheck> GetCheckSnapshots (Guid restaurantID);

	    IEnumerable<ICheck> GetAllChecks();
            
       IEnumerable<IEmployee> GetEmployeeSnapshots (Guid restaurantID);

	    IEnumerable<IEmployee> GetAllEmployees(); 
            
       IEnumerable<MenuRule> GetMenuRules (Guid restaurantID);

	    IEnumerable<MenuRule> GetAllMenuRules();
            
        Task<IEnumerable<Menu>> GetMenus (Guid restaurantID);

	    Task<IEnumerable<Menu>> GetAllMenus();
        
        IRestaurant GetRestaurantSnapshot (Guid restaurantID);

	    IEnumerable<IRestaurant> GetAllRestaurants();
	}
}

