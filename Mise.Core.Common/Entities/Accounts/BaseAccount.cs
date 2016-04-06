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
    public abstract class BaseAccount : EntityBase, IAccount
    {
        public abstract void When(IAccountEvent entityEvent);

        public abstract ICloneableEntity Clone();

        public abstract MiseAccountTypes AccountType { get;  }

        public virtual bool ContainsSearchString(string searchString)
        {
            return
                (PrimaryEmail != null && PrimaryEmail.ContainsSearchString(searchString))
                || (AccountHolderName != null && AccountHolderName.ContainsSearchString(searchString))
                || (Emails != null && Emails.Any(e => e.ContainsSearchString(searchString)))
                || (PhoneNumber != null && PhoneNumber.ContainsSearchString(searchString))
                ||
                (ReferralCodeForAccountToGiveOut != null &&
                 ReferralCodeForAccountToGiveOut.ContainsSearchString(searchString))
                || (ReferralCodeUsedToCreate != null && ReferralCodeUsedToCreate.ContainsSearchString(searchString))
                || (AccountType.ToString().Contains(searchString));
        }

        public EmailAddress PrimaryEmail { get; set; }
        public PersonName AccountHolderName { get; set; }

        public IEnumerable<EmailAddress> Emails { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        public ReferralCode ReferralCodeForAccountToGiveOut { get; set; }
        public ReferralCode ReferralCodeUsedToCreate { get; set; }
    }
}
