using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace MiseReporting.Models
{
    public class ReceivingOrderLineItemViewModel : BaseBeverageLineItemViewModel
    {
        public decimal? Price { get; set; }
        public decimal Quantity { get; set; }

        public ReceivingOrderLineItemViewModel(IReceivingOrderLineItem li) : base(li)
        {
            Price = li.LineItemPrice?.Dollars;
            Quantity = li.Quantity;
        }
    }
}
