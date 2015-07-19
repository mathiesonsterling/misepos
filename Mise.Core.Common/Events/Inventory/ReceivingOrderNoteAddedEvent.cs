using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
	public class ReceivingOrderNoteAddedEvent : BaseReceivingOrderEvent
	{
		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.ReceivingOrderNoteAdded;
			}
		}

		/// <summary>
		/// The note the user added
		/// </summary>
		/// <value>The notes.</value>
		public string Note{get;set;}
	}
}

