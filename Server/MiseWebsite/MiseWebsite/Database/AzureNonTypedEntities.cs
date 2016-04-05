using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiseWebsite.Models;

namespace MiseWebsite.Database
{
    public class AzureNonTypedEntities : DbContext
    {
        public AzureNonTypedEntities()
            : base("name=AzureNonTypedEntities")
        {
        }

        public virtual DbSet<AzureEntityStorage> AzureEntityStorages { get; set; }
        public virtual DbSet<AzureEventStorage> AzureEventStorages { get; set; }

        public virtual DbSet<SendEmailCSVFile> SendEmailCSVFiles { get; set; }

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
