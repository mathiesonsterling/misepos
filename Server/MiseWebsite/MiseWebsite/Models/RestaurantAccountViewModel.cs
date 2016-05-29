using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace MiseWebsite.Models
{
    public class RestaurantAccountViewModel
    {
        public RestaurantAccountViewModel() { }

        public RestaurantAccountViewModel(IBusinessAccount source)
        {
            EmailAddress = source.PrimaryEmail?.Value;
            BusinessName = source.BusinessName;
            ReferralCodeUsed = source.ReferralCodeUsedToCreate?.Code;
            ReferralCodeToGiveOut = source.ReferralCodeForAccountToGiveOut?.Code;
            AccountStatus = source.Status;
            CardNumber = source.CurrentCard?.MaskedCardNumber;
            ExpMonth = source.CurrentCard.ExpMonth.Value;
            ExpYear = source.CurrentCard?.ExpYear;
            BillingZip = source.CurrentCard?.BillingZip?.Value;
        }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmationPassword { get; set; }

        [Required]
        public string BusinessName { get; set; }
        public string ReferralCodeUsed { get; set; }
        public string ReferralCodeToGiveOut { get; set; }

        public MiseAccountStatus AccountStatus { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public int ExpMonth { get; set; }

        [Required]
        public int ExpYear { get; set; }

        [Required]
        public string BillingZip { get; set; }

        [Required]
        public string CardholderFirstName { get; set; }

        [Required]
        public string CardholderLastName { get; set; }

        [Required]
        public int CVC { get; set; }
    }
}