using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    public class VendorAccount : BaseAccount, IBusinessAccount
    {
        public override void When(IAccountEvent entityEvent)
        {
            throw new NotImplementedException();
        }

        public override ICloneableEntity Clone()
        {
            throw new NotImplementedException();
        }

        public override MiseAccountTypes AccountType => MiseAccountTypes.Vendor;

        public override bool ContainsSearchString(string searchString)
        {
            if (base.ContainsSearchString(searchString))
            {
                return true;
            }

            return (CurrentCard != null && CurrentCard.ContainsSearchString(searchString))
                   || PaymentPlan.ToString().Contains(searchString)
                   || (BusinessName != null && BusinessName.Contains(searchString));
        }

        public TimeSpan BillingCycle { get; set; }

        public CreditCard CurrentCard { get; set; }

        public MiseAccountStatus Status { get; set; }

        public MisePaymentPlan PaymentPlan { get; set; }
        public bool PaymentPlanSetupWithProvider { get; set; }

        public IEnumerable<MiseAppTypes> AppsOnAccount { get; set; }

        public IEnumerable<IAccountCharge> GetCharges()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccountPayment> GetPayments()
        {
            throw new NotImplementedException();
        }

        public string BusinessName { get; set; }

        public IEnumerable<Guid> VendorsManagedIds { get; set; } 
    }
}
