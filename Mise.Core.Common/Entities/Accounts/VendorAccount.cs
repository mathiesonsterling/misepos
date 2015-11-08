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
    public class VendorAccount : EntityBase, IBusinessAccount
    {
        public void When(IAccountEvent entityEvent)
        {
            throw new NotImplementedException();
        }

        public ICloneableEntity Clone()
        {
            throw new NotImplementedException();
        }

        public bool ContainsSearchString(string searchString)
        {
            throw new NotImplementedException();
        }

        public EmailAddress PrimaryEmail { get; set; }
        public PersonName AccountHolderName { get; set; }
        public IEnumerable<EmailAddress> Emails { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public TimeSpan BillingCycle { get; set; }
        public CreditCard CurrentCard { get; set; }
        public ReferralCode ReferralCodeForAccountToGiveOut { get; set; }
        public ReferralCode ReferralCodeUsedToCreate { get; set; }
        public MiseAccountStatus Status { get; set; }
        public MiseAccountTypes AccountType { get; set; }
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
