using System.Data.Entity;
using MiseWebsite.Models;

namespace MiseWebsite.Database.Implementation
{
    public class StronglyTypedEntitiesDbContext : DbContext
    {
        public StronglyTypedEntitiesDbContext()
            : base("name=StronglyTypedEntities")
        {
        }

        public virtual DbSet<SendEmailCSVFile> SendEmailCSVFiles { get; set; }

        public virtual DbSet<ResellerAccountStorage> ResellerAccounts { get; set; } 
    }
}