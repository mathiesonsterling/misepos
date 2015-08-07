using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Core.Entities;

namespace Mise.Core.Common.Entities.DTOs
{
    public class RestaurantEntityDataTransportObject : IEntityBase
    {
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public Guid ID { get; set; }
        public Guid? RestaurantID { get; set; }
        public EventID Revision { get; set; }

        /// <summary>
        /// The status of this object across all layers
        /// </summary>
        public ItemCacheStatus ItemCacheStatus { get; set; }

        /// <summary>
        /// JSON representation of the item
        /// </summary>
        public string JSON { get; set; }

        public Type SourceType { get; set; }

        public bool Equals(IEntityBase other)
        {
            return ID.Equals(other.ID) && Revision.Equals(other.Revision);
        }
    }
}
