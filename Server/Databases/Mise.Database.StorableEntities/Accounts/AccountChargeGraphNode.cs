using System;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.Accounts
{
    public class AccountChargeGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public string Revision { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }

        public DateTimeOffset DateStart { get; set; }
        public DateTimeOffset DateEnd { get; set; }
        public string AppChargedFor { get; set; }
        public decimal AmountChargedInDollars { get; set; }

        /// <summary>
        /// Store it here just in case!
        /// </summary>
        public Guid AccountID { get; set; }
        public AccountChargeGraphNode() { }

        public AccountChargeGraphNode(IAccountCharge source)
        {
            ID = source.ID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            DateStart = source.DateStart;
            DateEnd = source.DateEnd;
            AmountChargedInDollars = source.Amount.Dollars;
            AccountID = source.AccountID;
            AppChargedFor = source.App.ToString();
        }

        public AccountCharge Rehydrate(Guid accountID)
        {
            return new AccountCharge
            {
                ID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                DateStart = DateStart,
                DateEnd = DateEnd,
                Amount = new Money(AmountChargedInDollars),
                AccountID = accountID,
                App = (MiseAppTypes)Enum.Parse(typeof(MiseAppTypes), AppChargedFor)
            };
        }
    }
}
