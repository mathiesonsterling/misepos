using System;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Accounts
{
    public class AccountPaymentGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public string Revision { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }

        public decimal AmountPaidInDollars { get; set; }

        public string Status { get; set; }

        public string PaymentType { get; set; }

        public string ReferralCode { get; set; }

        /// <summary>
        /// Store it here just in case!
        /// </summary>
        public Guid AccountID { get; set; }

        public AccountPaymentGraphNode()
        {
        }


        public AccountPaymentGraphNode(IAccountPayment source)
        {
            ID = source.ID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            AmountPaidInDollars = source.Amount.Dollars;
            AccountID = source.AccountID;
            Status = source.Status.ToString();
            PaymentType = source.PaymentType.ToString();

            var down = source as AccountCredit;
            if (down != null && down.ReferralCodeGiven != null)
            {
                ReferralCode = down.ReferralCodeGiven.Code;
            }
        }

        public IAccountPayment Rehydrate(CreditCard card)
        {
            if (PaymentType == Core.ValueItems.PaymentType.InternalCreditCard.ToString())
            {
                return new AccountCreditCardPayment
                {
                    ID = ID,
                    AccountID = AccountID,
                    CreatedDate = CreatedDate,
                    LastUpdatedDate = LastUpdatedDate,
                    Revision = new EventID(Revision),

                    Amount = new Money(AmountPaidInDollars),
                    Status = (PaymentProcessingStatus) Enum.Parse(typeof (PaymentProcessingStatus), Status),
                    CardUsed = card
                };
            }

            if (PaymentType == Core.ValueItems.PaymentType.MiseCredit.ToString())
            {
                return new AccountCredit
                {
                    ID = ID,
                    CreatedDate = CreatedDate,
                    LastUpdatedDate = LastUpdatedDate,
                    Revision = new EventID(Revision),
                    Amount = new Money(AmountPaidInDollars),
                    ReferralCodeGiven = string.IsNullOrEmpty(ReferralCode) ? null : new ReferralCode(ReferralCode),
                    Status = (PaymentProcessingStatus) Enum.Parse(typeof (PaymentProcessingStatus), Status),
                };
            }

            throw new ArgumentException("Can't rehydrate for payment type " + PaymentType);
        }
    }
}
