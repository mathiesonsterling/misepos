using System;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public class CreditCardFailedAuthorizationEvent : BaseCreditCardEvent
	{
		public Money Amount { get; set;
		}

		public CreditCardAuthorizationCode AuthorizationCode {
			get;
			set;
		}

		#region implemented abstract members of BaseCheckEvent
		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.CreditCardFailedAuthorization;
			}
		}
		#endregion
	}
}

