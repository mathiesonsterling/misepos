using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Checks
{
	public class OrderItemSetMemoEvent : BaseCheckEvent
	{
		public override MiseEventTypes EventType
		{
		    get { return MiseEventTypes.OrderItemSetMemo; }
		}
			
		public string Memo {
			get;
			set;
		}

		public Guid OrderItemID{get;set;}

	}
}

