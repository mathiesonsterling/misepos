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
            PrimaryEmail = new EmailAddressDb {Value = source.PrimaryEmail?.Value};
            AccountPasswordHash = source.Password?.HashValue;
            AccountHolderFirstName = source.AccountHolderName?.FirstName;
            AccountHolderLastName = source.AccountHolderName?.LastName;
            Emails = source.Emails.Select(e => new EmailAddressDb { Value = e.Value}).ToList();
            AccountPhoneNumber = source.PhoneNumber?.Number;
            AccountPhoneNumberAreaCode = source.PhoneNumber?.AreaCode;
            ReferralCodeForAccountToGiveOut = source.ReferralCodeForAccountToGiveOut?.Code;
            ReferralCodeUsedToCreate = source.ReferralCodeUsedToCreate?.Code;
        } 

        public MiseAccountTypes AccountType { get; set; }

        public MiseAccountStatus Status { get; set; }
        public EmailAddressDb PrimaryEmail { get; set; }

        public string AccountPasswordHash { get; set; }

        public string AccountHolderFirstName { get; set; }
        public string AccountHolderLastName { get; set; }
        public List<EmailAddressDb> Emails { get; set; }

        public string AccountPhoneNumberAreaCode { get; set; }
        public string AccountPhoneNumber { get; set; }

        public string ReferralCodeForAccountToGiveOut { get; set; }

        public string ReferralCodeUsedToCreate { get; set; }


        protected abstract TConcrete CreateAccountSubobject();

        protected override TConcrete CreateConcreteSubclass()
        {
            var conc = CreateAccountSubobject();
            conc.Status = Status;
            conc.PrimaryEmail = PrimaryEmail.ToValueItem();
            conc.Password = new Core.ValueItems.Password {HashValue = AccountPasswordHash};
            conc.AccountHolderName = new PersonName(AccountHolderFirstName, AccountHolderLastName);
            conc.Emails = Emails.Select(e => e.ToValueItem()).ToList();
            conc.PhoneNumber = new PhoneNumber(AccountPhoneNumberAreaCode, AccountPhoneNumber);
            conc.ReferralCodeForAccountToGiveOut = new ReferralCode(ReferralCodeForAccountToGiveOut);
            conc.ReferralCodeUsedToCreate = new ReferralCode(ReferralCodeUsedToCreate);

            return conc;
        }
    }
}
