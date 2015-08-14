using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Restaurant
{
    /// <summary>
    /// Event marking that we want to add a new inventory section, that all our inventories will later use
    /// </summary>
    public class InventorySectionAddedToRestaurantEvent : BaseRestaurantEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.InventorySectionAddedToRestaurant; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public string SectionName { get; set; }
        public Guid SectionID { get; set; }
		public bool IsDefaultInventorySection{ get; set; }
		public bool AllowsPartialBottles{get;set;}
    }
}
