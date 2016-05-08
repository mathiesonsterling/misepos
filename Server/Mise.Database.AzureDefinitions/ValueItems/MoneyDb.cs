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
            Dollars = source.Dollars;
        }

        public decimal Dollars { get; set; }

        public Money ToValueItem()
        {
            return new Money(Dollars);
        }
    }
}
