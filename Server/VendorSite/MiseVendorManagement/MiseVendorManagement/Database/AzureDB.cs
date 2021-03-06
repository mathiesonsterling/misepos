using System.Data.Entity;

namespace MiseVendorManagement.Database
{
    public partial class AzureDB : DbContext
    {
        public AzureDB()
            : base("name=AzureDB")
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

        public System.Data.Entity.DbSet<MiseVendorManagement.Models.VendorItemForSaleViewModel> VendorItemForSaleViewModels { get; set; }
    }
}
