using System;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Entities;
namespace Mise.Core.Common
{
	public class UserSelectedRestaurant : BaseRestaurantEvent
	{
		#region implemented abstract members of BaseRestaurantEvent

		public override MiseEventTypes EventType {
			get {
				return MiseEventTypes.UserSelectedRestaurant;
			}
		}

		#endregion

		public Guid EmployeeID{get{return CausedById;}}
	}
}

