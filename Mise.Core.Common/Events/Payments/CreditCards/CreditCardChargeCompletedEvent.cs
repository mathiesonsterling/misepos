using Mise.Core.Entities;
using Mise.Core.ValueItems;


namespace Mise.Core.Common.Events.Payments.CreditCards
{
	public class CreditCardChargeCompletedEvent : BaseCreditCardEvent
	{
		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get { return MiseEventTypes.CreditCardChargeCompleted; }
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

		public CreditCardAuthorizationCode AuthorizationCode {
			get;
			set;
		}

		public bool WasAuthorized {
			get;
			set;
		}
			
	}
}

