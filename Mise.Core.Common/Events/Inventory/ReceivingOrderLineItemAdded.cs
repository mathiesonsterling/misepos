using System;

using Mise.Core.ValueItems.Inventory;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities;
using System.Collections.Generic;


namespace Mise.Core.Common.Events.Inventory
{
	public class ReceivingOrderLineItemAddedEvent : BaseReceivingOrderEvent, ICreateLineItemEvent
	{
		#region implemented abstract members of BaseReceivingOrderEvent

		public override MiseEventTypes EventType => MiseEventTypes.ReceivingOrderLineItemAdded;

	    public override bool IsEntityCreation => true;

	    #endregion

	    public Guid LineItemID { get; set; }

	    public string DisplayName{get;set;}

		/// <summary>
		/// Reconciled name in our system, so we can combat vendor's differning
		/// </summary>
		public string MiseName { get; set;}

		/// <summary>
		/// UPC for this item, if we know it
		/// </summary>
		public string UPC { get; set;}

		/// <summary>
		/// Size of the container it comes in, which also has other information in it
		/// </summary>
		public LiquidContainer Container { get; set;}

		/// <summary>
		/// If not null, this is how many units are in a case.  Allows us to convert cases to bottles
		/// </summary>
		/// <value>The size of the case.</value>
		public int? CaseSize{get;set;}

		public decimal Quantity{ get; set;}

		public IEnumerable<InventoryCategory> Categories{get;set;}
	}
}

