using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Server.Services.DAL
{
    /// <summary>
    /// Database that can store and retrieve entities
    /// </summary>
    public interface IEntityDAL
    {
        Task<IEnumerable<IAccount>> GetAccountsAsync();
        Task AddAccountAsync(IAccount account);
        Task UpdateAccountAysnc(IAccount account);


        Task<IRestaurant> GetRestaurantAsync(Guid restaurantID);
        Task<IEnumerable<IRestaurant>> GetRestaurantsAsync();
 
        Task AddRestaurantAsync(IRestaurant restaurant);
        Task UpdateRestaurantAsync(IRestaurant restaurant);


        /// <summary>
        /// Get the menus a restaurant has
        /// </summary>
        /// <param name="restaurantID"></param>
        /// <returns></returns>
        Task<IEnumerable<Menu>> GetMenusAsync(Guid restaurantID);

        Task<Menu> GetMenuByIDAsync(Guid menuID);

        Task AddMenuAsync(Menu menu);

        Task UpdateMenuAsync(Menu menu);


        Task<IEnumerable<MenuRule>> GetMenuRulesAsync(Guid restaurantID);
        Task AddMenuRuleAsync(MenuRule menuRule);
        Task UpdateMenuRuleAsync(MenuRule menuRule);
         
        /// <summary>
        /// Gets employees stored in the DB
        /// </summary>
        /// <param name="restaurantID">Restaurant to get for</param>
		/// <param name = "app">The application to retrieve for</param>
        /// <returns></returns>
        Task<IEnumerable<IEmployee>> GetEmployeesAsync(Guid restaurantID);
        /// <summary>
        /// Gets all the employees who use the inventory app
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IEmployee>> GetInventoryAppUsingEmployeesAsync(); 
        Task AddEmployeeAsync(IEmployee employee);
        Task UpdateEmployeeAsync(IEmployee employee);


        Task<IEnumerable<IInventory>> GetInventoriesAsync(Guid restaurantID);
        Task<IEnumerable<IInventory>> GetInventoriesAsync(DateTimeOffset dateSince);
 
        Task AddInventoryAsync(IInventory inventory);
        Task UpdateInventoryAsync(IInventory inventory);

        Task UpdateVendorAsync(IVendor newVersion);
        Task AddVendorAsync(IVendor newVersion);
        Task<IEnumerable<IVendor>> GetVendorsAsync();
        Task UpdatePurchaseOrderAsync(IPurchaseOrder newVersion);
        Task AddPurchaseOrderAsync(IPurchaseOrder newVersion);
        Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersAsync(DateTimeOffset maxTimeBack);
        Task<IEnumerable<IPurchaseOrder>> GetPurchaseOrdersAsync(Guid restaurantID); 

        Task UpdateReceivingOrderAsync(IReceivingOrder arg);
        Task AddReceivingOrderAsync(IReceivingOrder arg);
        Task<IEnumerable<IReceivingOrder>>  GetReceivingOrdersAsync(DateTimeOffset maxTimeBack);
        Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(Guid restaurantID);
        Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersAsync(IVendor vendor); 

        Task UpdatePARAsync(IPAR arg);
        Task AddPARAsync(IPAR arg);
        Task<IEnumerable<IPAR>> GetPARsAsync(Guid restaurantID);
        Task<IEnumerable<IPAR>> GetPARsAsync();
        Task<IEnumerable<LiquidContainer>> GetAllLiquidContainersAsync();

        Task<IEnumerable<IApplicationInvitation>> GetOpenApplicationInvitations(EmailAddress destination);
        Task<IEnumerable<IApplicationInvitation>> GetApplicationInvitations(); 
        Task AddApplicationInvitiation(IApplicationInvitation invite);
        Task UpdateApplicationInvitation(IApplicationInvitation invite);
        Task UpdatePARLineItemAsync(IPARBeverageLineItem lineItem);
        Task DeletePARLineItemAsync(Guid lineItemID);
        Task SetLineItemsForPARAsync(IPARBeverageLineItem lineItem, Guid parID);
        Task CreateCategories(ICollection<ICategory> categories);
    }
}
