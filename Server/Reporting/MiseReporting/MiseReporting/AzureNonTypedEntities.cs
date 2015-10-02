namespace MiseReporting
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AzureNonTypedEntities : DbContext
    {
        public AzureNonTypedEntities()
            : base("name=AzureNonTypedEntities")
        {
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

        public System.Data.Entity.DbSet<MiseReporting.Models.PurchaseOrderViewModel> PurchaseOrderViewModels { get; set; }

        public System.Data.Entity.DbSet<MiseReporting.Models.EmployeeViewModel> EmployeeViewModels { get; set; }

        public System.Data.Entity.DbSet<MiseReporting.Models.RestaurantViewModel> RestaurantViewModels { get; set; }
    }
}
