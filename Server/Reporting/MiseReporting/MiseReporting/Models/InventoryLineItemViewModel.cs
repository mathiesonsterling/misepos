using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace MiseReporting.Models
{
    public class InventoryLineItemViewModel : BaseBeverageLineItemViewModel
    {
        public decimal Total { get; set; }
        public decimal FullBottles { get; set; }
        public decimal PartialBottles { get; set; }

        public InventoryLineItemViewModel()
        {
            
        }

        public InventoryLineItemViewModel(IInventoryBeverageLineItem li) : base(li)
        {
            Total = li.Quantity;
            FullBottles = li.NumFullBottles;
            PartialBottles = li.NumPartialBottles;
        }
    }
}
