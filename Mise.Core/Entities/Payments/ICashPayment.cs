using System;

using Mise.Core.ValueItems;
namespace Mise.Core.Entities.Payments
{
	public interface ICashPayment : IPayment
	{
		/// <summary>
		/// Amount of cash that was given
		/// </summary>
		/// <value>The amount tendered.</value>
		Money AmountTendered{get;}

		/// <summary>
		/// Change given back
		/// </summary>
		/// <value>The change given.</value>
		Money ChangeGiven{get;}
	}
}

