using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class PhoneNumber : Core.ValueItems.PhoneNumber, IDbValueItem<Core.ValueItems.PhoneNumber>
    {
        public Core.ValueItems.PhoneNumber ToValueItem()
        {
            return this;
        }
    }
}
