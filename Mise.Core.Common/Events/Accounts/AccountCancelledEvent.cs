using System;

using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant.Events;
namespace Mise.Core.Common.Events.Accounts
{
    public class AccountCancelledEvent : BaseAccountEvent, IRestaurantEvent
    {
        #region implemented abstract members of BaseAccountEvent
        public override MiseEventTypes EventType
        {
            get
            {
                return MiseEventTypes.AccountCancelled;
            }
        }
        #endregion

        public override Guid RestaurantId{get;set;}
    }
}

