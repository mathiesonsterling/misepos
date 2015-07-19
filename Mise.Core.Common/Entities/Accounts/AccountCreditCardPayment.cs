using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    public class AccountCreditCardPayment : EntityBase, IAccountPayment, ICloneableEntity, ITextSearchable
    {
        public AccountCreditCardPayment()
        {
            Status = PaymentProcessingStatus.Empty;
            Amount = Money.None;
        }

        public Guid AccountID { get; set; }
        public Money Amount { get; set; }
        public PaymentType PaymentType { get{return PaymentType.InternalCreditCard;}}

        public PaymentProcessingStatus Status { get; set; }
        public CreditCard CardUsed { get; set; }

        public ICloneableEntity Clone()
        {
            var item = CloneEntityBase(new AccountCreditCardPayment());
            item.AccountID = AccountID;
            item.Amount = Amount;
            item.Status = Status;
            item.CardUsed = CardUsed;

            return item;
        }

        public bool ContainsSearchString(string searchString)
        {
            return Status.ToString().Contains(searchString)
                   || CardUsed.ContainsSearchString(searchString)
                   || CreatedDate.ToString().Contains(searchString);
        }
    }
}
