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
    public class ResellerAccount : EntityBase, IResellerAccount
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

        public EmailAddress PrimaryEmail { get; }

        public PersonName AccountHolderName { get; }

        public IEnumerable<EmailAddress> Emails { get; }

        public PhoneNumber PhoneNumber { get; }
        public CreditCard CurrentCard { get; }
        public ReferralCode ReferralCodeForAccountToGiveOut { get; }
        public ReferralCode ReferralCodeUsedToCreate { get; }
        public MiseAccountStatus Status { get; }
        public MiseAccountTypes AccountType { get; }
        public MisePaymentPlan PaymentPlan { get; }
        public bool PaymentPlanSetupWithProvider { get; }
        public IEnumerable<MiseAppTypes> AppsOnAccount { get; }
        public IEnumerable<IAccountCharge> GetCharges()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccountPayment> GetPayments()
        {
            throw new NotImplementedException();
        }

        public Guid? ResellerUnderId { get; set; }
    }
}
