using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Accounts
{
    public class AccountHasPaymentPlanSetupEvent : BaseAccountEvent
    {
        public override MiseEventTypes EventType => MiseEventTypes.AccountHasPaymentPlanSetup;
        public MisePaymentPlan PaymentPlan { get; set; }
    }
}
