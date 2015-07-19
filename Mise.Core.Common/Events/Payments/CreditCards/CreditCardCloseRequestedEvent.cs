using System;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Check.Events;

namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public class CreditCardCloseRequestedEvent : BaseCreditCardEvent
	{
		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.CreditCardCloseRequested;
			}
		}

		#endregion

		public Money AmountPaid {
			get;
			set;
		}

		public Money TipAmount {
			get;
			set;
		}

		public CreditCardAuthorizationCode CodeFromAuthorization {
			get;
			set;
		}
			
	}
}

