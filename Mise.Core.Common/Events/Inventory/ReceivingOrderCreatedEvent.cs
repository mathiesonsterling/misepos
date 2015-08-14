using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Inventory
{
    public class ReceivingOrderCreatedEvent : BaseReceivingOrderEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.ReceivingOrderCreated; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public override bool IsAggregateRootCreation
        {
            get { return true; }
        }

        /// <summary>
		/// ID of the vendor this is for
		/// </summary>
		/// <value>The vendor I.</value>
		public Guid VendorID{get;set;}
    }
}
