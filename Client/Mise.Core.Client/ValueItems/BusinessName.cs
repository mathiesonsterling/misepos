using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.ValueItems
{
    public class BusinessName : Core.ValueItems.BusinessName, IDbValueItem<Core.ValueItems.BusinessName>
    {
        public BusinessName() { }

        public BusinessName(Core.ValueItems.BusinessName source) : base(source.FullName, source.ShortName)
        {
        }

	    public BusinessName(string fullName) :base(fullName){ }

        public Core.ValueItems.BusinessName ToValueItem()
        {
            return this;
        }
    }
}
