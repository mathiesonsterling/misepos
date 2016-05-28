using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.ValueItems
{
    public class ReferralCodeDb : IDbValueItem<Core.ValueItems.ReferralCode>
    {
        public string Code { get; set; }

        public Core.ValueItems.ReferralCode ToValueItem()
        {
            return new Core.ValueItems.ReferralCode {Code = Code};
        }
    }
}
