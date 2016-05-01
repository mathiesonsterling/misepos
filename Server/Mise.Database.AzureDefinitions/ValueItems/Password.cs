using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class Password : Core.ValueItems.Password, IDbValueItem<Core.ValueItems.Password>
    {
        public Core.ValueItems.Password ToValueItem()
        {
            return this;
        }
    }
}
