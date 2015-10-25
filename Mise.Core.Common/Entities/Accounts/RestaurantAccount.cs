using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    /// <summary>
    /// Represents an account with us
    /// </summary>
    public class RestaurantAccount : EntityBase, IAccount
    {
        public RestaurantAccount()
        {
            Charges = new List<AccountCharge>();
            Payments = new List<AccountCreditCardPayment>();
            AccountCredits = new List<AccountCredit>();
        }

        public EmailAddress PrimaryEmail { get; set; }
        public PersonName AccountHolderName { get; set; }
        public IEnumerable<EmailAddress> Emails { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        public virtual TimeSpan BillingCycle { get; set; }

        public CreditCard CurrentCard { get; set; }

        public ReferralCode ReferralCodeUsedToCreate { get; set; }
        public ReferralCode ReferralCodeForAccountToGiveOut { get; set; }


        public MiseAccountStatus Status { get; set; }

        public virtual MiseAccountTypes AccountType
        {
            get { return MiseAccountTypes.Restaurant; }
        }

        public MisePaymentPlan PaymentPlan
        {
            get;
            set;
        }

        public IEnumerable<MiseAppTypes> AppsOnAccount { get; set; }

        public ICollection<AccountCharge> Charges { get; set; } 

        public virtual IEnumerable<IAccountCharge> GetCharges()
        {
            return Charges;
        }

        public ICollection<AccountCreditCardPayment> Payments { get; set; }
        public ICollection<AccountCredit> AccountCredits { get; set; } 

        public virtual IEnumerable<IAccountPayment> GetPayments()
        {
            var list = new List<IAccountPayment>();
            list.AddRange(Payments);
            list.AddRange(AccountCredits);
            return list;
        }

        public bool IsActive
        {
            get
            {
                return Status == MiseAccountStatus.Active
                       || Status == MiseAccountStatus.Overdue
                       || Status == MiseAccountStatus.CancelledButStillActive;
            }
        }

        public ICloneableEntity Clone()
        {
            var item = CloneEntityBase(new RestaurantAccount());
            item.AccountHolderName = AccountHolderName;
            item.Emails = Emails.Select(e => e).ToList();
            item.PhoneNumber = PhoneNumber;
            item.PrimaryEmail = PrimaryEmail;
            item.ReferralCodeForAccountToGiveOut = ReferralCodeForAccountToGiveOut;
            item.ReferralCodeUsedToCreate = ReferralCodeUsedToCreate;
            item.Status = Status;
            item.CurrentCard = CurrentCard;
            item.Payments = Payments.Select(p => p).ToList();
            item.Charges = Charges.Select(c => c).ToList();
            item.AccountCredits = AccountCredits.Select(c => c).ToList();
            item.BillingCycle = BillingCycle;
            return item;
        }

        public void When(IAccountEvent entityEvent)
        {
            switch (entityEvent.EventType)
            {
                case MiseEventTypes.AccountRegisteredFromMobileDevice:
                    WhenRegisteredFromMobile((AccountRegisteredFromMobileDeviceEvent) entityEvent);
                    break;
                default:
                    throw new InvalidOperationException("Don't know how to handle " + entityEvent.EventType);
            }

            LastUpdatedDate = entityEvent.CreatedDate;
            Revision = entityEvent.EventOrder;
        }

        protected virtual void WhenRegisteredFromMobile(AccountRegisteredFromMobileDeviceEvent ev)
        {
            Id = ev.AccountID;
            PrimaryEmail = ev.Email;
            AccountHolderName = ev.AccountHolderName;
            BillingCycle = new TimeSpan(30, 0, 0, 0);
            CurrentCard = ev.CreditCard;
            Emails = new List<EmailAddress> {ev.Email};
            PhoneNumber = ev.PhoneNumber;
            Status = MiseAccountStatus.NotActivated;
            PaymentPlan = ev.PaymentPlan;
            if (ev.ReferralCode != null)
            {
                
            }
        }

        public bool ContainsSearchString(string searchString)
        {
            return (PrimaryEmail != null && PrimaryEmail.ContainsSearchString(searchString))
                   || (AccountHolderName != null && AccountHolderName.ContainsSearchString(searchString))
                   || (CurrentCard != null) && CurrentCard.ContainsSearchString(searchString)
                   || (Emails != null && Emails.Any(e => e.ContainsSearchString(searchString)))
                   || (PhoneNumber != null && PhoneNumber.ContainsSearchString(searchString))
                   || (ReferralCodeForAccountToGiveOut != null && ReferralCodeForAccountToGiveOut.ContainsSearchString(searchString))
                   || (ReferralCodeUsedToCreate != null && ReferralCodeUsedToCreate.ContainsSearchString(searchString))
                   || (Payments.Any(p => p.ContainsSearchString(searchString)))
                   || (AccountCredits.Any(p => p.ContainsSearchString(searchString)))
                   || (Charges.Any(p => p.ContainsSearchString(searchString)));
        }
    }
}
