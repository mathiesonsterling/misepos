using System.Data.Entity;

namespace MiseWebsite.Database.Implementation
{
    public class AzureNonTypedEntitiesDbContext : DbContext
    {
        public AzureNonTypedEntitiesDbContext()
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
    }
}
