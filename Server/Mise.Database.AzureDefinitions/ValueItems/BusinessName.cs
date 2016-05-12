using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
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
