using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a section in the Inventory, that matches a restaurant section
    /// </summary>
    public interface IInventorySection : IRestaurantEntityBase, ICloneableEntity, ITextSearchable
    {
		/// <summary>
		/// ID of the inventory this belongs to
		/// </summary>
		/// <value>The inventory I.</value>
		Guid InventoryID{get;}

        string Name { get; }

		/// <summary>
		/// If true, we've done inventory for this section
		/// </summary>
		/// <value><c>true</c> if completed; otherwise, <c>false</c>.</value>
		bool Completed{get;}

		/// <summary>
		/// If set, the employee that is currently working on this section.  Used for multimachine setups to warn
		/// </summary>
		/// <value>The currently in use by.</value>
		Guid? CurrentlyInUseBy{get;}
		/// <summary>
		/// Last time the count for this section was started
		/// </summary>
		/// <value>The time count started.</value>
		DateTimeOffset? TimeCountStarted{get;}

        /// <summary>
        /// The employee ID of the last person who completed this section
        /// </summary>
        Guid? LastCompletedBy { get;}

        /// <summary>
        /// Points back to the inventory section of the restaurant
        /// </summary>
        Guid RestaurantInventorySectionID { get; }

        IEnumerable<IInventoryBeverageLineItem> GetInventoryBeverageLineItemsInSection();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetNextItemPosition();
    }
}
