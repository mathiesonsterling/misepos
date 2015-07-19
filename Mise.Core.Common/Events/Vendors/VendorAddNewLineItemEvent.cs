using System;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities;
namespace Mise.Core.Common
{
	public class VendorAddNewLineItemEvent : BaseVendorEvent, ICreateLineItemEvent
	{
		#region implemented abstract members of BaseVendorEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.VendorLineItemAdded;
			}
		}

		#endregion

	    public Guid LineItemID { get; set; }

	    public string DisplayName { get; set; }

		/// <summary>
		/// Reconciled name in our system, so we can combat vendor's differning
		/// </summary>
		public string MiseName { get; set; }

		/// <summary>
		/// UPC for this item, if we know it
		/// </summary>
		public string UPC { get; set; }

		/// <summary>
		/// Size of the container it comes in, which also has other information in it
		/// </summary>
		public LiquidContainer Container { get; set; }

		/// <summary>
		/// If not null, this is how many units are in a case.  Allows us to convert cases to bottles
		/// </summary>
		/// <value>The size of the case.</value>
		public int? CaseSize { get; set; }

		public int? Quantity { get; set; }
	}
}

