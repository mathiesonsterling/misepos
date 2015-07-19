using System;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
namespace Mise.Core.Entities.Base
{
	/// <summary>
	/// Base of events that form our event source model.  
	/// </summary>
	public interface IEntityEventBase
	{
		/// <summary>
		/// The type of event this is, held as a string for JSON serialization and consumption
		/// </summary>
		/// <value>The type of the event.</value>
		MiseEventTypes EventType {get;}

		/// <summary>
		/// Unique ID for the event
		/// </summary>
		Guid ID{get;}

        /// <summary>
        /// The restaurant where this event was generated.
        /// </summary>
        Guid RestaurantID { get; }

		/// <summary>
		/// Ordering of the event.  This is in order and unique, but only for terminal and restaurant. 
		/// </summary>
		/// 
		EventID EventOrderingID{ get; }

		/// <summary>
		/// The ID of the item that caused this event
		/// </summary>
		Guid CausedByID{get;}

		/// <summary>
		/// Date this event was created
		/// </summary>
		/// <value>The created date.</value>
		DateTimeOffset CreatedDate{ get; }

		/// <summary>
		/// The ID of the device the event occurred on
		/// </summary>
		/// <value>The device I.</value>
		string DeviceID{get;set;}
	}
}

