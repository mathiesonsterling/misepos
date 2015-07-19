using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Inventory
{
    /// <summary>
    /// Represents a beverage item, whether from a vendor, in the inventory, desired in the PAR, etc
    /// </summary>
    public interface IBaseBeverageLineItem : IEntityBase, ITextSearchable, ITaggable
    {
		string DisplayName{get;}

        /// <summary>
        /// Reconciled name in our system, so we can combat vendor's differning
        /// </summary>
        string MiseName { get; }

        /// <summary>
        /// UPC for this item, if we know it
        /// </summary>
        string UPC { get; }

        /// <summary>
        /// Size of the container it comes in, which also has other information in it
        /// </summary>
        LiquidContainer Container { get; }

		/// <summary>
		/// If not null, this is how many units are in a case.  Allows us to convert cases to bottles
		/// </summary>
		/// <value>The size of the case.</value>
		int? CaseSize{get;}

		/// <summary>
		/// Get the categories this item is listed under
		/// </summary>
		IEnumerable<ICategory> GetCategories();
	}
}
