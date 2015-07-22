using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Reports;

namespace Mise.Inventory.Reports
{
    public class CompletedInventoryReport : BaseReport
    {
        private readonly IInventory _inventory;

        public CompletedInventoryReport(IInventory inventory)
        {
            if (inventory == null)
            {
                throw new ArgumentException("Inventory is null in report");
            }
            if (inventory.DateCompleted.HasValue == false)
            {
                throw new ArgumentException("Inventory is not yet complete!");
            }
            _inventory = inventory;
        }

        public override ReportResult RunReport()
        {
            var dic = new Dictionary<string, ReportResultLineItem>();

            //get all the items, and combine their amounts 
            foreach (var li in _inventory.GetBeverageLineItems())
            {
                //make our key
                var key = li.DisplayName + "|" + li.MiseName + "|" + li.Container.DisplayName;
                if (dic.ContainsKey(key) == false)
                {
                    var newItem = new ReportResultLineItem(li.DisplayName, li.Container.DisplayName, li.Quantity,
                        li.Quantity >= 0);
                    dic.Add(key, newItem);
                }
                else
                {
                    var existing = dic[key];
                    existing.Quantity += li.Quantity;
                    existing.IsErrored = existing.Quantity >= 0;
                }
            }

            //make the title
            var title = "Inventory ran " + _inventory.DateCompleted;

            return new ReportResult(ReportTypes.CompletedInventory, title, dic.Values);
        }
    }
}
