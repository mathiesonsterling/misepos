using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Accounts
{
	/// <summary>
	/// An account with MisePOS
	/// </summary>
	/// <remarks>Should probably be </remarks>
	public interface IAccount : IEventStoreEntityBase<IAccountEvent>, ITextSearchable
	{
		/// <summary>
		/// Primary email for this account
		/// </summary>
		/// <value>The primary email.</value>
		EmailAddress PrimaryEmail{ get; }

        Password Password { get; }

        PersonName AccountHolderName { get; }

		/// <summary>
		/// All emails associated with this account
		/// </summary>
		/// <value>The emails.</value>
		IEnumerable<EmailAddress> Emails{get;}

		PhoneNumber PhoneNumber{get;}

		/// <summary>
		/// The referral code this account uses
		/// </summary>
		/// <value>The referral code.</value>
		ReferralCode ReferralCodeForAccountToGiveOut{ get; }

        /// <summary>
        /// If there, the referral code that we created the account with
        /// </summary>
        ReferralCode ReferralCodeUsedToCreate { get; }

	    MiseAccountTypes AccountType { get; }
	    MiseAccountStatus Status { get; set; }
	}
}

