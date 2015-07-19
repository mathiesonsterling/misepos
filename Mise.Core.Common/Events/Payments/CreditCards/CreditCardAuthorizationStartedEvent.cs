using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Payments.CreditCards
{
    /// <summary>
    /// Class signals when the user requested the authorization
    /// </summary>
    public class CreditCardAuthorizationStartedEvent : BaseCreditCardEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.CreditCardAuthorizationStarted; }
        }
    }
}
