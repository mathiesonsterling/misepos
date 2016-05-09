using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class Weight : Core.ValueItems.Weight, IDbValueItem<Core.ValueItems.Weight>
    {
        public Weight() { }

        public Weight(Core.ValueItems.Weight source) : base(source) { } 

        public Core.ValueItems.Weight ToValueItem()
        {
            return this;
        }
    }
}
