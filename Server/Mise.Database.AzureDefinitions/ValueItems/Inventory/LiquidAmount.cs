using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems.Inventory
{
    [ComplexType]
    public class LiquidAmount : IDbValueItem<Core.ValueItems.Inventory.LiquidAmount>
    {
        public decimal Milliliters { get; set; }

        /// <summary>
        /// The density of the liquid - used to allow us to measure it by weight
        /// </summary>
        public decimal? SpecificGravity { get; set; }

        public Core.ValueItems.Inventory.LiquidAmount ToValueItem()
        {
            return new Core.ValueItems.Inventory.LiquidAmount
            {
                Milliliters = Milliliters,
                SpecificGravity = SpecificGravity
            };
        }
    }
}
