using System;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards
{
	public interface ICreditCardEvent : ICheckEvent
	{
		/// <summary>
		/// Credit card used in this event
		/// </summary>
		/// <value>The credit card.</value>
		CreditCard CreditCard{get;}

		Guid PaymentID{get;}
	}
}

