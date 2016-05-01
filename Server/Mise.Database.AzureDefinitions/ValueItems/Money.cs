using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class Money : Core.ValueItems.Money, IDbValueItem<Core.ValueItems.Money>
    {
        public Core.ValueItems.Money ToValueItem()
        {
            return this;
        }
    }
}
