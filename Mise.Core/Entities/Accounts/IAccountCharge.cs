using System;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Accounts
{
    /// <summary>
    /// Represents a charge made to an account
    /// </summary>
    public interface IAccountCharge : IEntityBase
    {
        /// <summary>
        /// The app this charge is for
        /// </summary>
        MiseAppTypes App { get; }
        Guid AccountID { get; }

        Money Amount { get; }

        /// <summary>
        /// The period for which the account is being charged
        /// </summary>
        DateTimeOffset DateStart { get; }
        DateTimeOffset DateEnd { get; }
    }
}
