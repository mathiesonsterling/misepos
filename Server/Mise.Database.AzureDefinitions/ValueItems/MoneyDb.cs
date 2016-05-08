using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class MoneyDb :  IDbValueItem<Core.ValueItems.Money>
    {
        public decimal Dollars { get; set; }

        public Core.ValueItems.Money ToValueItem()
        {
            return new Core.ValueItems.Money(Dollars);
        }
    }
}
