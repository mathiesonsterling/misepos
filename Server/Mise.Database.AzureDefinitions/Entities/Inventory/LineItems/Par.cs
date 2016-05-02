using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    public class Par : BaseDbEntity<IPar, Core.Common.Entities.Inventory.Par>
    {
        protected override Core.Common.Entities.Inventory.Par CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.Par
            {
                CreatedByEmployeeID = CreatedByEmployeeID,
                IsCurrent = IsCurrent,
                ParLineItems = ParLineItems.Select(pl => pl.ToBusinessEntity()).ToList()
            };
        }

        public Guid CreatedByEmployeeID { get; set; }
        public bool IsCurrent { get; set; }

        public List<ParBeverageLineItem> ParLineItems { get; set; }
    }
}
