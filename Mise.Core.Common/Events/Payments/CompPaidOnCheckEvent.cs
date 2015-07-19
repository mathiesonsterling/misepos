using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Entities.People.Events;



namespace Mise.Core.Common.Events.Payments
{
	/// <summary>
	/// Event marking that we took a direct amount off a check, instead of an item
	/// </summary>
	public class CompPaidDirectlyOnCheckEvent : BaseCheckEvent, IEmployeeEvent
	{
		public Money Amount {
			get;
			set;
		}
			
		#region implemented abstract members of BaseCheckEvent

		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.CompPaidDirectlyOnCheck;
			}
		}
		#endregion
	}
}

