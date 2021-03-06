﻿using System;
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
    public class RestaurantAccount : BaseAccount, IBusinessAccount
    {
        public RestaurantAccount()
        {
            Charges = new List<AccountCharge>();
            Payments = new List<AccountCreditCardPayment>();
            AccountCredits = new List<AccountCredit>();
        }

        public virtual TimeSpan BillingCycle { get; set; }

        public CreditCard CurrentCard { get; set; }


        public override MiseAccountTypes AccountType => MiseAccountTypes.Restaurant;

        public MisePaymentPlan PaymentPlan
        {
            get;
            set;
        }

        public bool PaymentPlanSetupWithProvider
        {
            get;
            set;
        }

        [Obsolete]
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

        public bool IsActive => Status == MiseAccountStatus.Active
                                || Status == MiseAccountStatus.Overdue
                                || Status == MiseAccountStatus.CancelledButStillActive;

        public override ICloneableEntity Clone()
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

        public override void When(IAccountEvent entityEvent)
        {
            switch (entityEvent.EventType)
            {
                case MiseEventTypes.AccountRegisteredFromMobileDevice:
                    WhenRegisteredFromMobile((AccountRegisteredFromMobileDeviceEvent) entityEvent);
                    break;
                case MiseEventTypes.AccountHasPaymentPlanSetup:
                    WhenAccountHasPaymentPlanSetup((AccountHasPaymentPlanSetupEvent) entityEvent);
                    break;
                case MiseEventTypes.AccountCancelled:
                    WhenAccountCancelled((AccountCancelledEvent)entityEvent);
                    break;
                default:
                    throw new InvalidOperationException("Don't know how to handle " + entityEvent.EventType);
            }

            LastUpdatedDate = entityEvent.CreatedDate;
            Revision = entityEvent.EventOrder;
        }

        private void WhenAccountCancelled(AccountCancelledEvent ec)
        {
            Status = MiseAccountStatus.Cancelled;
        }

        private void WhenAccountHasPaymentPlanSetup(AccountHasPaymentPlanSetupEvent entityEvent)
        {
            PaymentPlanSetupWithProvider = true;
            PaymentPlan = entityEvent.PaymentPlan;
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
                ReferralCodeUsedToCreate = ev.ReferralCode;
            }
        }

        public override bool ContainsSearchString(string searchString)
        {
            if (base.ContainsSearchString(searchString))
            {
                return true;
            }
            return  (CurrentCard != null) && CurrentCard.ContainsSearchString(searchString)
                   || (PhoneNumber != null && PhoneNumber.ContainsSearchString(searchString))
                   || (Payments.Any(p => p.ContainsSearchString(searchString)))
                   || (AccountCredits.Any(p => p.ContainsSearchString(searchString)))
                   || (Charges.Any(p => p.ContainsSearchString(searchString)))
                   || (BusinessName != null && BusinessName.Contains(searchString));
        }

        public string BusinessName { get; set; }
    }
}
