using System.ComponentModel.DataAnnotations.Schema;
using Mise.Core.ValueItems;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class MoneyDb :  IDbValueItem<Money>
    {
        public MoneyDb() { }

        public MoneyDb(Money source)
        {
            if (source != null)
            {
                Dollars = decimal.Round(source.Dollars, 3);
            }
        }

        public decimal Dollars { get; set; }

        public Money ToValueItem()
        {
            return new Money(Dollars);
        }
    }
}
