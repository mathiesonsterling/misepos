using System;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;

namespace Mise.Core
{
	/// <summary>
	/// Represents any event that deals with an item being comped on a check
	/// </summary>
	public interface IItemCompedEvent : ICheckEvent, IEmployeeEvent
	{
		/// <summary>
		/// The order item ID of this
		/// </summary>
		/// <value>The order item I.</value>
		Guid OrderItemID{get;}
		/// <summary>
		/// Amount of money this comp represents
		/// </summary>
		/// <value>The amount.</value>
		Money Amount{get;}
	}
}

