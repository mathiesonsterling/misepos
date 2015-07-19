using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Common.Events
{
    public interface ICreateLineItemEvent : IEntityEventBase
    {
        Guid LineItemID { get; set; }

        string DisplayName { get; set; }

        /// <summary>
        /// Reconciled name in our system, so we can combat vendor's differning
        /// </summary>
        string MiseName { get; set; }

        /// <summary>
        /// UPC for this item, if we know it
        /// </summary>
        string UPC { get; set; }
    }
}
