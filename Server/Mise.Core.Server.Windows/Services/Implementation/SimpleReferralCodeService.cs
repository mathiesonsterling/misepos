using System;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Core.Server.Windows.Services.Implementation
{
    /// <summary>
    /// Very simple implementation, does not check for existing!
    /// </summary>
    public class SimpleReferralCodeService : IReferralCodeService
    {
        public ReferralCode GenerateReferralCodeForAccount(IAccount acct)
        {
            var bAcct = acct as IBusinessAccount;
            if (bAcct == null)
            {
                throw new ArgumentException("Can only work with a business account!");
            }

            var name = bAcct.BusinessName;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Business does not have ");
            }

            var code = name.Length < 6 ? name : name.Substring(0, 6);
            return new ReferralCode(code.ToUpper());
        }
    }
}
