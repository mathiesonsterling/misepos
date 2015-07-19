using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Entities.People.Events;

namespace Mise.Core.Common.Events.Restaurant
{
	public class NewRestaurantRegisteredOnAppEvent : BaseRestaurantEvent, IEmployeeEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.NewRestaurantRegisteredOnApp; }
        }

		public Guid EmployeeID {
			get;
			set;
		}

        public MiseAppTypes ApplicationUsed { get; set; }
        public RestaurantName Name { get; set; }
        public StreetAddress StreetAddress { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
    }
}
