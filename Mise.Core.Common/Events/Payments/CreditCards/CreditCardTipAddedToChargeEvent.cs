using System;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Payments;
namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public class CreditCardTipAddedToChargeEvent : BaseCreditCardEvent
	{
		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.CredtCardTipAddedToCharge;
			}
		}
		public Money TipAmount {
			get;
			set;
		}

		public CreditCardAuthorizationCode AuthorizationCode {
			get;
			set;
		}
	}
}

