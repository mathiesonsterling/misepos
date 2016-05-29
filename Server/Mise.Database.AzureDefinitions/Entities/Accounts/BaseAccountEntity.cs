using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;
using Mise.Database.AzureDefinitions.ValueItems;
using ReferralCode = Mise.Core.ValueItems.ReferralCode;

namespace Mise.Database.AzureDefinitions.Entities.Accounts
{
    public abstract class BaseAccountEntity<TEntity, TConcrete> : BaseDbEntity<TEntity, TConcrete> 
        where TEntity : IEntityBase, IAccount
        where TConcrete : BaseAccount, TEntity
    {
        protected BaseAccountEntity()
        {
            
        }

        protected BaseAccountEntity(TEntity source) :base(source)
        {
            AccountType = source.AccountType;
            Status = source.Status;
            PrimaryEmail = source.PrimaryEmail?.Value;
            AccountPasswordHash = source.Password?.HashValue;
            AccountHolderFirstName = source.AccountHolderName?.FirstName;
            AccountHolderLastName = source.AccountHolderName?.LastName;
            AccountPhoneNumber = source.PhoneNumber?.Number;
            AccountPhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
            ReferralCodeForAccountToGiveOut = source.ReferralCodeForAccountToGiveOut?.Code;
            ReferralCodeUsedToCreate = source.ReferralCodeUsedToCreate?.Code;

            var emails = source.Emails.Where(e => e != null).Select(e => e.Value);
            Emails = string.Join(",", emails);
        } 

        public MiseAccountTypes AccountType { get; set; }

        public MiseAccountStatus Status { get; set; }
        public string PrimaryEmail { get; set; }

        public string AccountPasswordHash { get; set; }

        public string AccountHolderFirstName { get; set; }
        public string AccountHolderLastName { get; set; }

        public string Emails { get; set; }

        public string AccountPhoneNumberAreaCode { get; set; }
        public string AccountPhoneNumber { get; set; }

        public string ReferralCodeForAccountToGiveOut { get; set; }

        public string ReferralCodeUsedToCreate { get; set; }


        protected abstract TConcrete CreateAccountSubobject();

        protected override TConcrete CreateConcreteSubclass()
        {
            var conc = CreateAccountSubobject();
            conc.Status = Status;
            conc.PrimaryEmail = new EmailAddress(PrimaryEmail);
            conc.Password = new Password {HashValue = AccountPasswordHash};
            conc.AccountHolderName = new PersonName(AccountHolderFirstName, AccountHolderLastName);
            conc.PhoneNumber = new PhoneNumber(AccountPhoneNumberAreaCode, AccountPhoneNumber);
            conc.ReferralCodeForAccountToGiveOut = new ReferralCode(ReferralCodeForAccountToGiveOut);
            conc.ReferralCodeUsedToCreate = new ReferralCode(ReferralCodeUsedToCreate);

            conc.Emails = Emails.Split(',').Select(e => new EmailAddress(e)).ToList();
            return conc;
        }
    }
}
