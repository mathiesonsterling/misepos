using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Accounts
{
    /// <summary>
    /// Account for a business using Mise (vendor, restaurant)
    /// </summary>
    public interface IBusinessAccount : IAccount
    {
        string BusinessName { get; }

        MiseAccountStatus Status { get; }
        MiseAccountTypes AccountType { get; }
        MisePaymentPlan PaymentPlan { get; }
        /// <summary>
        /// If true, we've setup the billing with our provider.  If not, we still need to!
        /// </summary>
        bool PaymentPlanSetupWithProvider { get; }

        IEnumerable<MiseAppTypes> AppsOnAccount { get; }

        //payments
        IEnumerable<IAccountCharge> GetCharges();

        //charges
        IEnumerable<IAccountPayment> GetPayments();

        CreditCard CurrentCard { get; }
    }
}
