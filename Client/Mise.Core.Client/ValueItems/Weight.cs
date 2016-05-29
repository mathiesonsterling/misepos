using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.ValueItems
{
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
