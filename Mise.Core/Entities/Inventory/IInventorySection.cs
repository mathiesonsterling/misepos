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
        string Name { get; }

		/// <summary>
		/// If true, we've done inventory for this section
		/// </summary>
		/// <value><c>true</c> if completed; otherwise, <c>false</c>.</value>
		bool Completed{get;}

        /// <summary>
        /// The employee ID of the last person who completed this section
        /// </summary>
        Guid? LastCompletedBy { get; set; }


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
