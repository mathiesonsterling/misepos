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
    /// <summary>
    /// Credit given to an account by Mise or others
    /// </summary>
    public class AccountCredit : EntityBase, IAccountPayment, ICloneableEntity, ITextSearchable
    {
        private PaymentProcessingStatus _status;
        public Guid AccountID { get; set; }
        public Money Amount { get; set; }
        public PaymentType PaymentType { get { return PaymentType.MiseCredit; } }

        public PaymentProcessingStatus Status
        {
            get { return _status; }
            set
            {
                if (value != PaymentProcessingStatus.CreditNeedsProcessing &&
                    value != PaymentProcessingStatus.CreditProcessed)
                {
                    throw new ArgumentException("Invalid payment type given for a credit!");
                }
                _status = value;
            }
        }

        /// <summary>
        /// If here, this is the referral code we were given to get this credit
        /// </summary>
        public ReferralCode ReferralCodeGiven { get; set; }
        public ICloneableEntity Clone()
        {
            var item = CloneEntityBase(new AccountCredit());
            item.AccountID = AccountID;
            item.Amount = Amount;
            item.ReferralCodeGiven = ReferralCodeGiven;

            return item;
        }

        public bool ContainsSearchString(string searchString)
        {
            return Status.ToString().Contains(searchString)
                                   || CreatedDate.ToString().Contains(searchString);
        }
    }
}
