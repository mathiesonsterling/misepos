using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Vendors;
using Mise.Core.ValueItems;
using MiseWebsite.Models;

namespace MiseWebsite.Database
{
    public interface IManagementDAL
    {
        Task<IEnumerable<IInventory>> GetInventoriesForRestaurant(Guid restaurantId);
        Task<IEnumerable<IInventory>> GetInventoriesCompletedAfter(DateTimeOffset date);
        Task<IInventory> GetInventoryById(Guid invId);
        Task<IInventoryBeverageLineItem> GetInventoryLineItem(Guid inventoryId, Guid lineItemId);
        Task UpdateInventoryLineItem(Guid inventoryId, IInventoryBeverageLineItem li);
        Task UpdateInventory(IInventory inv);
        Task UpdateRestaurant(IRestaurant rest);
        Task<IEnumerable<IRestaurant>> GetAllRestaurants();
        Task<IRestaurant> GetRestaurantById(Guid restaurantId);
        Task<IEnumerable<IRestaurant>> GetRestaurantsUnderAccont(IAccount acct);
        Task InsertRestaurant(Restaurant rest);
        Task DeleteRestaurant(Guid id);
        Task<IEnumerable<IEmployee>> GetAllEmployees();
        Task<IEmployee> GetEmployeeById(Guid id);
        Task<IEmployee> GetEmployeeWhoCreatedInventory(IInventory inv);
        Task<IEnumerable<IEmployee>> GetAllEmployeesContaining(string search);
        Task<IEmployee> GetEmployeeWithEmail(EmailAddress email);
        Task<IEmployee> GetEmployeeWithEmailAndPassword(EmailAddress email, string password);
        Task CreateEmployee(Employee emp);
        Task UpdateEmployee(Employee emp);
        Task<IEnumerable<IReceivingOrder>> GetReceivingOrdersForRestaurant(Guid restaurantId);
        Task<IReceivingOrder> GetReceivingOrderById(Guid id);
        Task<IPurchaseOrder> GetPurchaseOrderById(Guid id);
        Task<IPar> GetParById(Guid id);
        Task<IVendor> GetVendorById(Guid id);
        Task<IEnumerable<EmailAddress>> GetEmailToSendReportToForRestaurant(Guid restaurantId);
        Task<IAccount> GetAccountById(Guid id);
        Task<IEnumerable<IAccount>> GetAccountsByEmail(EmailAddress email);
        Task<IEnumerable<IAccount>> GetRestaurantAccounts(string searchString);
        Task<IEnumerable<IAccount>> GetAccountsWaitingForPaymentPlan();
        Task UpdateAccount(IAccount account);
        Task<IEnumerable<SendEmailCSVFile>> GetLastSentEmails(int numBack);
        Task<SendEmailCSVFile> GetEmailForEntity(Guid entityId);
        Task<IEnumerable<SendEmailCSVFile>> GetUnsentEmails();
        Task CreateEmailRecord(SendEmailCSVFile email);
        Task MarkEmailAsSent(SendEmailCSVFile email);
    }
}