using System;
using Mise.Core.Entities.Base;

using Mise.Core.ValueItems;
namespace Mise.Core.Entities.Payments
{
	/// <summary>
	/// Represents the basic 
	/// </summary>
	public interface IPayment : IRestaurantEntityBase
	{
		/// <summary>
		/// Represents the amount of money this payment should give
		/// when figuring out if a check is paid or not
		/// </summary>
		/// <value>The amount to apply to check total.</value>
		Money AmountToApplyToCheckTotal{get;}

		/// <summary>
		/// Show how much the payment was on our list
		/// </summary>
		/// <value>The display amount.</value>
		Money DisplayAmount{get;}

		/// <summary>
		/// The check this payment was made on
		/// </summary>
		/// <value>The check I.</value>
		Guid CheckID{get;}

		/// <summary>
		/// Employee tat took the payment
		/// </summary>
		/// <value>The employee I.</value>
		Guid EmployeeID{get;}

		PaymentType PaymentType{get;}

		/// <summary>
		/// If true, we open the cash drawer when we're recieved
		/// </summary>
		/// <value><c>true</c> if opens cash drawer; otherwise, <c>false</c>.</value>
		bool OpensCashDrawer{get;}

        /// <summary>
        /// Return a deep copy
        /// </summary>
        /// <returns></returns>
	    IPayment Clone();
	}
}

