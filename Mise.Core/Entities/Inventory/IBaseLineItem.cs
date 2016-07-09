using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Entities.Inventory
{
    public interface IBaseLineItem : IEntityBase, ITextSearchable, ITaggable
    {
        string DisplayName { get; }

        /// <summary>
        /// Reconciled name in our system, so we can combat vendor's differning
        /// </summary>
        string MiseName { get; }

        /// <summary>
        /// UPC for this item, if we know it
        /// </summary>
        string UPC { get; }

        decimal Quantity { get; }

        /// <summary>
        /// If not null, this is how many units are in a case.  Allows us to convert cases to bottles
        /// </summary>
        /// <value>The size of the case.</value>
        int? CaseSize { get; }

        /// <summary>
        /// Get the categories this item is listed under
        /// </summary>
        IEnumerable<ICategory> GetCategories();

    }
}
