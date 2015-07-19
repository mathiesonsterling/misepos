using System;
using System.Linq;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Entities;
using Mise.Core.Entities.Check.Events.PaymentEvents;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;
using System.Collections.Generic;


namespace Mise.Core.Common.Events.Payments
{
	public class MarkCheckAsPaidEvent : BaseCheckEvent
	{
		public bool IsSplitPayment {
			get;
			set;
		}


		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.MarkCheckAsPaid;
			}
		}
	}
}

