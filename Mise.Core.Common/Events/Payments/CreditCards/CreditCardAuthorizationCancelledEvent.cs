using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
using System;

namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public class CreditCardAuthorizationCancelledEvent : BaseCreditCardEvent
	{
		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.CreditCardCancelledEvent;
			}
		}
		#endregion

		public CreditCardAuthorizationCode AuthorizationCode {
			get;
			set;
		}
		public bool WasRolledBack {
			get;
			set;
		}
	}
}

