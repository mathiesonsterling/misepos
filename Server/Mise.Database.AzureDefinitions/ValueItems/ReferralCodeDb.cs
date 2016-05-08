using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class ReferralCodeDb : IDbValueItem<Core.ValueItems.ReferralCode>
    {
        public string Code { get; set; }

        public Core.ValueItems.ReferralCode ToValueItem()
        {
            return new Core.ValueItems.ReferralCode {Code = Code};
        }
    }
}
