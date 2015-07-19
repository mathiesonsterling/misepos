using System;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
namespace Mise.Core.Common
{
	public class PARLineItemQuantityUpdatedEvent : BasePAREvent
	{
		#region implemented abstract members of BasePAREvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.PARLineItemQuantityUpdated;
			}
		}

		#endregion

		public Guid LineItemID{get;set;}
		public int UpdatedQuantity{get;set;}
	}
}

