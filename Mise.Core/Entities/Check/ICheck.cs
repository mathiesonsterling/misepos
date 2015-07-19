using System;
using System.Collections.Generic;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Payments;
namespace Mise.Core.Entities.Check
{


    /// <summary>
    /// Represents a single check - all that we've ordered.  May want to have this split table, etc
    /// </summary>
    public interface ICheck : ICloneableEntity, IEventStoreEntityBase<ICheckEvent>, IRestaurantEntityBase
    {
        IEnumerable<OrderItem> OrderItems { get; }

		void AddOrderItem (OrderItem item);
		void RemoveOrderItem (OrderItem item);

		/// <summary>
		/// ID of the server that created this check
		/// </summary>
		Guid CreatedByServerID { get; set;}

		/// <summary>
		/// The last server to modify this order
		/// </summary>
		/// <value>The last touched server I.</value>
		Guid LastTouchedServerID{ get; set;}

		/// <summary>
		/// Customer who is ordering this check.  Can there be more than 1?
		/// </summary>
		/// <value>The customer.</value>
        Customer Customer { get; set; }

        /// <summary>
        /// The time the check was closed
        /// </summary>
		DateTime? ClosedDate { get; set;}

        string DisplayName { get; }

        string GetTopOfCheck();

        CheckPaymentStatus PaymentStatus { get; set;}

        /// <summary>
        /// Causes us to recalculate our payment status, based upon what payments we've been given
        /// </summary>
        void UpdatePaymentStatusFromPayments();


		/// <summary>
		/// If set, there's a credit card already associated with this order
		/// </summary>
		/// <value>The credit card.</value>
		IEnumerable<CreditCard> CreditCards{get;}

		Money Total { get; }

		/// <summary>
		/// Amount of change due on this check
		/// </summary>
		/// <value>The change due.</value>
		Money ChangeDue{ get; set;}

        /// <summary>
        /// Gets all the payments that have been applied to this check
        /// </summary>
        IEnumerable<IPayment> GetPayments();

        void AddPayment(IPayment payment);
        bool RemovePayment(IPayment payment);

        /// <summary>
        /// Gets all the discounts that have been applied
        /// </summary>
        /// <value>The discounts.</value>
        IEnumerable<IDiscount> GetDiscounts();

        void AddDiscount(IDiscount discount);
        bool RemoveDiscount(IDiscount discount);
    }
}
