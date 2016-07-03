using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferMiseEntitesTool.Database
{
    class NewEntityDB : DbContext
    {
        public NewEntityDB() : base("name=MS_TableConnectionString")
        {
            
        }

        public virtual DbSet<AzureEntityStorage> AzureEntityStorages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AzureEntityStorage>()
                .Property(e => e.Version).IsFixedLength();
        }
    }
}
