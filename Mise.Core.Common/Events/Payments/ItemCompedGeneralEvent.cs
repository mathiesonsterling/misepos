using System;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Payments
{
	/// <summary>
	/// Event made when we comp an item for a general reason off the check
	/// </summary>
	public class ItemCompedGeneralEvent : BaseCheckEvent, IItemCompedEvent
	{
		#region implemented abstract members of BaseCheckEvent
		public override MiseEventTypes EventType {
		    get
		    {
		        return MiseEventTypes.ItemCompedGeneral;
		    }
		}
		#endregion

		public Guid OrderItemID {
			get;
			set;
		}

		public string Reason {
			get;
			set;
		}

		public Money Amount{get;set;}
	}
}

