using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace MiseReporting.Models
{
    public class InventoryLineItemViewModel : BaseBeverageLineItemViewModel
    {
        [DisplayName("Section")]
        public string SectionName { get; set; }

        [DisplayName("Total in Bottles")]
        public decimal Total { get; set; }

        [DisplayName("Number Full")]
        public decimal FullBottles { get; set; }

        [DisplayName("Number Partial")]
        public decimal PartialBottles { get; set; }

        [DisplayName("Price Paid")]
        public decimal? PricePaid { get; set; }

        public InventoryLineItemViewModel()
        {
            
        }

        public InventoryLineItemViewModel(string sectionName, IInventoryBeverageLineItem li) : base(li)
        {
            SectionName = sectionName;
            Total = li.Quantity;
            FullBottles = li.NumFullBottles;
            PartialBottles = li.NumPartialBottles;
            PricePaid = li.PricePaid?.Dollars;
        }
    }
}
