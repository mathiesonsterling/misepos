using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;

namespace MiseReporting.Models
{
    public abstract class BaseBeverageLineItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UPC { get; set; }

        [DisplayName("Container")]
        public string ContainerName { get; set; }

        [DisplayName("Size ML")]
        public decimal ContainerSizeML { get; set; }

        public string Categories { get; set; }

        protected BaseBeverageLineItemViewModel() { }

        protected BaseBeverageLineItemViewModel(IBaseBeverageLineItem source)
        {
            Id = source.ID;
            Name = source.DisplayName;
            UPC = source.UPC;
            if (source.Container != null)
            {
                ContainerName = source.Container.DisplayName;
                ContainerSizeML = source.Container.AmountContained.Milliliters;
            }

            var allCatNames = source.GetCategories().Select(c => c.Name);
            Categories = string.Join(",", allCatNames);
        }
    }
}
