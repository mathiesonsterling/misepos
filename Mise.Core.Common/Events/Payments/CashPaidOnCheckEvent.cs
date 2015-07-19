using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Payments
{
	public class CashPaidOnCheckEvent : BaseCheckEvent
	{
		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get { return MiseEventTypes.CashPaidOnCheck; }
		}
			
		#endregion

		public Money AmountPaid{ get; set;}

		/// <summary>
		/// Amount of money the customer gave
		/// </summary>
		/// <value>The amount tendered.</value>
		public Money AmountTendered{ get; set;}

		/// <summary>
		/// Change we gave to the customer
		/// </summary>
		/// <value>The change given.</value>
		public Money ChangeGiven{ get; set;}
	}
}

