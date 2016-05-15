using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server.Tables;
using Mise.Database.AzureDefinitions.Entities;
using Mise.Database.AzureDefinitions.Entities.Accounts;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.People;
using Mise.Database.AzureDefinitions.Entities.Restaurant;
using Mise.Database.AzureDefinitions.Entities.Vendor;
using Mise.Database.AzureDefinitions.ValueItems;
using RestaurantAccount = Mise.Database.AzureDefinitions.Entities.Accounts.RestaurantAccount;

namespace Mise.Database.AzureDefinitions.Context
{
    public class StockboyMobileAppServiceContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string CONNECTION_STRING_NAME = "Name=MS_TableConnectionString";

        public StockboyMobileAppServiceContext() : base(CONNECTION_STRING_NAME)
        {
        } 

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Par> Pars { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<ReceivingOrder> ReceivingOrders { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<MiseEmployeeAccount> MiseEmployeeAccounts { get; set; }
        public DbSet<RestaurantAccount> RestaurantAccounts { get; set; }
        public DbSet<ApplicationInvitation> ApplicationInvitations { get; set; }

        public DbSet<MiseApplication> MiseApplications { get; set; }
        public DbSet<EmailAddressDb> Emails { get; set; }
        public DbSet<RestaurantInventorySection> RestaurantInventorySections { get; set; }
        public DbSet<InventoryCategory> InventoryCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }
    }

}
