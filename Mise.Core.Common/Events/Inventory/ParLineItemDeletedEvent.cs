using System;

using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	public class ParLineItemDeletedEvent : BasePAREvent
	{
		#region implemented abstract members of BasePAREvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ParLineItemDeleted;
			}
		}

		#endregion
		public Guid LineItemId{get;set;}
	}
}

