using System;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
namespace Mise.Core.Common.Events.Restaurant
{
    public class RestaurantReportingEmailSetEvent : BaseRestaurantEvent
    {
        #region implemented abstract members of BaseRestaurantEvent
        public override MiseEventTypes EventType
        {
            get
            {
                return MiseEventTypes.RestaurantReportingEmailSet;
            }
        }
        #endregion

        public EmailAddress Email{get;set;}
    }
}

