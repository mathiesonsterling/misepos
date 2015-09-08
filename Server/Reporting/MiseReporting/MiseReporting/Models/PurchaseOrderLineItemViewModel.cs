using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace MiseReporting.Models
{
    public class PurchaseOrderLineItemViewModel : BaseBeverageLineItemViewModel
    {
        public decimal Quantity { get; set; }

        public PurchaseOrderLineItemViewModel(IPurchaseOrderLineItem li) : base(li)
        {
            Quantity = li.Quantity;
        }
    }
}
