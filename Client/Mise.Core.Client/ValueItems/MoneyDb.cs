using Mise.Core.ValueItems;
using System;
namespace Mise.Core.Client.ValueItems
{
    public class MoneyDb :  IDbValueItem<Money>
    {
        public MoneyDb() { }

        public MoneyDb(Money source)
        {
            if (source != null)
            {
                Dollars = Math.Round(source.Dollars, 3);
            }
        }

        public decimal Dollars { get; set; }

        public Money ToValueItem()
        {
            return new Money(Dollars);
        }
    }
}
