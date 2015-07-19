using System;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.Entities.Payments;


namespace Mise.Core.Common.Events.Payments
{
	/// <summary>
	/// Event when we take a discount or grat off a check
	/// </summary>
	public class DiscountRemovedFromCheckEvent : BaseCheckEvent
	{
		public Guid DiscountID {
			get;
			set;
		}


		public override MiseEventTypes EventType {
			get
			{
			    return MiseEventTypes.DiscountRemovedFromCheck;
			}
		}
	}
}

