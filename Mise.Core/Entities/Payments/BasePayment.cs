using System;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Payments
{
	public abstract class BasePayment : RestaurantEntityBase, IPayment
	{
	    public abstract IPayment Clone();
        public abstract PaymentType PaymentType { get; }

		public abstract Money AmountToApplyToCheckTotal {get;}

		public abstract Money DisplayAmount {
			get;
		}

		public abstract bool OpensCashDrawer { get;}

		public Guid CheckID{get;set;}
			
		public Guid EmployeeID{get;set;}

		protected BasePayment(){
			ID = Guid.NewGuid ();
		}
	}
}

