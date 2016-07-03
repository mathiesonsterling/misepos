using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Database.AzureDefinitions.Context;
using Mise.Database.AzureDefinitions.Entities.People;

namespace TransferMiseEntitesTool
{
    public class PopulateRestIdsInEmployee
    {
        public async Task Execute()
        {
            using (var db = new StockboyMobileAppServiceContext())
            {
                var allRels = db.EmployeeRestaurantRelationships
                    .Include(r => r.Employee)
                    .Include(r => r.Restaurant)
                    .ToList();

                var emps = db.Employees.ToList();
                foreach (var emp in db.Employees)
                {
                    var rels = allRels.Where(er => er.Employee != null && er.Employee.EntityId == emp.EntityId).ToList();
                    if (rels.Any())
                    {
                        var restIds = rels.Select(re => re.Restaurant.EntityId.ToString());
                        var idString = string.Join(",", restIds);

                        emp.RestaurantsEmployedAtIds = idString;

                        db.Entry(emp).State = EntityState.Modified;
                    }
                }
                await db.SaveChangesAsync();
            }
        }
    }
}
