using System;
using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Restaurant
{
    /// <summary>
    /// This is a restaurant event, to set the restaurant as a placeholder and create. 
    /// An employee event to associate teh employee
    /// an inventory event to create a new, empty current inventory
    /// </summary>
    public class PlaceholderRestaurantCreatedEvent : BaseRestaurantEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.PlaceholderRestaurantCreated; }
        }

		/// <summary>
		/// The App that this is being set for
		/// </summary>
		/// <value>The app.</value>
		public MiseAppTypes App{get;set;}

    }
}
