using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Accounts
{
    public sealed class AccountGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }

        public string AccountHolderFirstName { get; set; }
        public string AccountHolderMiddleName { get; set; }
        public string AccountHolderLastName { get; set; }

        public string PhoneNumberAreaCode { get; set; }
        public string PhoneNumberNumber { get; set; }

        public string AccountType { get; set; }
        public TimeSpan BillingCycle { get; set; }

        public IEnumerable<string> AppsOnAccount { get; set; }
 

        /// <summary>
        /// If our account is active or not
        /// </summary>
        public string Status { get; set; }

        public AccountGraphNode()
        {
            
        }

        public AccountGraphNode(IAccount source)
        {
            ID = source.ID;
            Revision = source.Revision.ToDatabaseString();
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;

            AccountType = source.AccountType.ToString();
            AccountHolderFirstName = source.AccountHolderName.FirstName;
            AccountHolderMiddleName = source.AccountHolderName.MiddleName;
            AccountHolderLastName = source.AccountHolderName.LastName;

            PhoneNumberAreaCode = source.PhoneNumber.AreaCode;
            PhoneNumberNumber = source.PhoneNumber.Number;

            BillingCycle = source.BillingCycle;
            AppsOnAccount = source.AppsOnAccount.Select(app => app.ToString());

            Status = source.Status.ToString();
        }

        public IAccount Rehydrate(IEnumerable<EmailAddress> emails, CreditCard currentCard, IEnumerable<AccountCharge> charges, 
            IEnumerable<AccountCreditCardPayment> ccPayments, IEnumerable<AccountCredit> refCredits, ReferralCode referralCodeToGiveOut, ReferralCode referralCodeCreatedWith)
        {
            if (AccountType == MiseAccountTypes.Restaurant.ToString())
            {
                var acc = new RestaurantAccount
                {
                    ID = ID,
                    CreatedDate = CreatedDate,
                    LastUpdatedDate = LastUpdatedDate,
                    Revision = new EventID(Revision),

                    PhoneNumber = new PhoneNumber {AreaCode = PhoneNumberAreaCode, Number = PhoneNumberNumber},
                    BillingCycle = BillingCycle,
                    CurrentCard = currentCard,
                    ReferralCodeForAccountToGiveOut = referralCodeToGiveOut,
                    ReferralCodeUsedToCreate = referralCodeCreatedWith,
                    Status = (MiseAccountStatus) Enum.Parse(typeof (MiseAccountStatus), Status),
                    AppsOnAccount = AppsOnAccount.Select(a => (MiseAppTypes)Enum.Parse(typeof(MiseAppTypes), a)),
                    Charges = charges.ToList(),
                    Payments = ccPayments.ToList(),
                    AccountCredits = refCredits.ToList(),

                    Emails = emails,


                };

                return acc;
            }
            if (AccountType == MiseAccountTypes.MiseEmployee.ToString())
            {
                return new MiseEmployeeAccount
                {
                    ID = ID,
                    CreatedDate = CreatedDate,
                    LastUpdatedDate = LastUpdatedDate,
                    Revision = new EventID(Revision),

                    PhoneNumber = new PhoneNumber {AreaCode = PhoneNumberAreaCode, Number = PhoneNumberNumber},
                    BillingCycle = BillingCycle,
                    CurrentCard = currentCard,
                    ReferralCodeForAccountToGiveOut = referralCodeToGiveOut,
                    ReferralCodeUsedToCreate = referralCodeCreatedWith,
                    Status = (MiseAccountStatus) Enum.Parse(typeof (MiseAccountStatus), Status),
                    AppsOnAccount = AppsOnAccount.Select(a => (MiseAppTypes) Enum.Parse(typeof (MiseAppTypes), a)),
                    Emails = emails,
                };
            }

            throw new ArgumentException("Can't rehydrate account of type " + AccountType);
        }
    }
}
