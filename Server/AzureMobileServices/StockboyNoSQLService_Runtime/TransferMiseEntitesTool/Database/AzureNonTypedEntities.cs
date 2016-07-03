using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferMiseEntitesTool.Database
{
    public class AzureNonTypedEntities : DbContext
    {
        public AzureNonTypedEntities()
            : base("name=AzureNonTypedEntities")
        {
    
        }

        public virtual DbSet<ExistingAzureEntity> AzureEntityStorages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExistingAzureEntity>()
                .Property(e => e.Version)
                .IsFixedLength();
        }
    }
}
