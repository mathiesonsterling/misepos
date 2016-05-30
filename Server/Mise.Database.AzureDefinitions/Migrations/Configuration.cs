namespace Mise.Database.AzureDefinitions.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.StockboyMobileAppServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Mise.Database.AzureDefinitions.Context.StockboyMobileAppServiceContext";
        }

        protected override void Seed(Context.StockboyMobileAppServiceContext context)
        {
            foreach (var restSec in context.RestaurantInventorySections.Include(rs => rs.Restaurant)) 
            {
                if (restSec.Restaurant != null)
                {
                    restSec.RestaurantId = restSec.Restaurant.EntityId;
                    context.Entry(restSec).State = EntityState.Modified;
                }
            }
            context.SaveChanges();

            foreach (var invSec in context.InventorySections.Include(iv => iv.Inventory))
            {
                if (invSec.Inventory != null)
                {
                    invSec.InventoryId = invSec.Inventory.EntityId;
                    context.Entry(invSec).State = EntityState.Modified;
                }
            }
            context.SaveChanges();

            var invSecIds = context.InventorySections.Select(i => i.EntityId).Distinct().ToList();
            foreach (var invSec in invSecIds)
            {
                var lis =
                    context.InventoryBeverageLineItems.Include(i => i.InventorySection)
                        .Where(li => li.InventorySection != null && li.InventorySection.EntityId == invSec);
                foreach (var li in lis)
                {
                    li.InventorySectionId = invSec;
                    context.Entry(li).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
