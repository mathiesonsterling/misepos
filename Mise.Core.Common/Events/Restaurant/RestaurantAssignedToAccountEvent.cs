using System;

using Mise.Core.Entities;
namespace Mise.Core.Common.Events.Restaurant
{
    public class RestaurantAssignedToAccountEvent : BaseRestaurantEvent
    {
        #region implemented abstract members of BaseRestaurantEvent
        public override MiseEventTypes EventType
        {
            get
            {
                return MiseEventTypes.RestaurantAssignedToAccount;
            }
        }
        #endregion

        public Guid AccountId{get;set;}
    }
}

