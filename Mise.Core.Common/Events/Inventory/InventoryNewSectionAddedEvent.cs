using System;
using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Inventory
{
	public class InventoryNewSectionAddedEvent : BaseInventoryEvent
	{
		#region implemented abstract members of BaseInventoryEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.InventoryNewSectionAdded;
			}
		}

		#endregion

        public Guid SectionID { get; set; }

		/// <summary>
		/// Name of the section
		/// </summary>
		/// <value>The name.</value>
		public string Name{get;set;}

		/// <summary>
		/// ID of the matching section for the restaurant
		/// </summary>
		/// <value>The restaurant section identifier.</value>
		public Guid RestaurantSectionId{get;set;}
	}
}

