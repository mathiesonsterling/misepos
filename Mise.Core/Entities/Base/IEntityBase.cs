using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Base
{
    public interface IEntityBase : IEquatable<IEntityBase>
    {
        DateTimeOffset CreatedDate { get; }
        DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Unique ID for the entity, in a DDD sense
        /// </summary>
        /// <value>The I.</value>
        Guid Id { get; }

        /// <summary>
        /// The event number that this entity represents up to - the last event that is reflected here
        /// </summary>
        /// <value>The revision.</value>
        EventID Revision { get; set; }
    }
}
