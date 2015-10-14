using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using MiseReporting.Models;

namespace MiseReporting
{
    public class AzureNonTypedEntities : IdentityDbContext<ApplicationUser>
    {
        public AzureNonTypedEntities()
            : base("name=AzureNonTypedEntities")
        {
        }

        public static AzureNonTypedEntities Create()
        {
            return new AzureNonTypedEntities();
        }

        public virtual DbSet<AzureEntityStorage> AzureEntityStorages { get; set; }
        public virtual DbSet<AzureEventStorage> AzureEventStorages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AzureEntityStorage>()
                .Property(e => e.Version)
                .IsFixedLength();

            modelBuilder.Entity<AzureEventStorage>()
                .Property(e => e.Version)
                .IsFixedLength();
        }

        public DbSet<PurchaseOrderViewModel> PurchaseOrderViewModels { get; set; }

        public DbSet<EmployeeViewModel> EmployeeViewModels { get; set; }

        public DbSet<RestaurantViewModel> RestaurantViewModels { get; set; }

        public DbSet<InventoryLineItemViewModel> InventoryLineItemViewModels { get; set; }
    }
}
