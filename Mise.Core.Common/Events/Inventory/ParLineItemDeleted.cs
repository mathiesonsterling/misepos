using System;

namespace Mise.Core.Common.Events.Inventory
{
	public class ParLineItemDeleted : BasePAREvent
	{
		#region implemented abstract members of BasePAREvent

		public override Mise.Core.Entities.MiseEventTypes EventType {
			get {
				throw new NotImplementedException ();
			}
		}

		#endregion

		
		public Guid LineItemId{get;set;}
	}
}

