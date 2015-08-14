using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Accounts
{
    public class AccountRegisteredFromMobileDeviceEvent : BaseAccountEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.AccountRegisteredFromMobileDevice; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public override bool IsAggregateRootCreation
        {
            get { return true; }
        }

        public EmailAddress Email { get; set; }

        public PersonName AccountHolderName { get; set; }
        public CreditCard CreditCard { get; set; }

		public MiseAppTypes AppType{ get; set;}
        public PhoneNumber PhoneNumber { get; set; }

        /// <summary>
        /// Note this is a referral code we're USING, not one that we've been given!
        /// </summary>
        public ReferralCode ReferralCode { get; set; }

        public MiseAccountTypes AccountType { get; set; }
    }
}
