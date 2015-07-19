using System;
using System.Linq;
using System.Security;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Check;
namespace Mise.Core.Common.Events.Payments
{
	public class DiscountAppliedToCheckEvent : BaseCheckEvent
	{
	    public DiscountPercentage DiscountPercentage { get; set; }
        public DiscountAmount DiscountAmount { get; set; }
        public DiscountPercentageAfterMinimumCashTotal DiscountPercentageAfterMinimumCashTotal { get; set; }

		public IDiscount Discount {
		    get
		    {
		        if (DiscountPercentageAfterMinimumCashTotal != null)
		        {
		            return DiscountPercentageAfterMinimumCashTotal;
		        }
		        if (DiscountPercentage != null)
		        {
		            return DiscountPercentage;
		        }
		        if (DiscountAmount != null)
		        {
		            return DiscountAmount;
		        }
		        return null;
		    }
		}


		public override MiseEventTypes EventType {
			get { return MiseEventTypes.DiscountAppliedToCheck; }
		}
	}
}

