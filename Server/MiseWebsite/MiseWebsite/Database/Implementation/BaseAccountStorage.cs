using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Accounts;

namespace MiseWebsite.Database.Implementation
{
    public abstract class BaseAccountStorage : BaseEntityStorage
    {
        protected BaseAccountStorage()
        {
            
        }

        protected BaseAccountStorage(IAccount source) : base(source)
        {
            PrimaryEmail = source.PrimaryEmail?.Value;
            FirstName = source.AccountHolderName?.FirstName;
            MiddleName = source.AccountHolderName?.MiddleName;
            LastName = source.AccountHolderName?.LastName;
           // Emails = source.Emails?.Select(e => e?.Value).ToList();
            AreaCode = source.PhoneNumber?.AreaCode;
            PhoneNumber = source.PhoneNumber?.Number;
            ReferralCodeCreatedWith = source.ReferralCodeUsedToCreate?.Code;
            ReferralCodeToGiveOut = source.ReferralCodeForAccountToGiveOut?.Code;
            PasswordHash = source.Password?.HashValue;
        }

        public string PrimaryEmail { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string PasswordHash { get; set; }

        /// <summary>
        /// TODO figure out how to get emails to store
        /// </summary>
        /*
        public List<string> Emails { get; set; }*/

        public string AreaCode { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferralCodeToGiveOut { get; set; }
        public string ReferralCodeCreatedWith { get; set; }
    }
}