using System;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;

namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public class CreditCardAuthorizedEvent : BaseCreditCardEvent
	{

		public Money Amount {
			get;
			set;
		}

		public CreditCardAuthorizationCode AuthorizationCode {
			get;
			set;
		}

		public bool WasAuthorized { get; set;}

		#region implemented abstract members of BaseCheckEvent
		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.CreditCardAuthorized;
			}
		}
		#endregion


	}
}

