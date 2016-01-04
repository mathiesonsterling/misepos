using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;

namespace MiseReporting.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Look up all accounts that haven't yet created a payment plan, then set them up
        /// </summary>
        /// <returns></returns>
        Task CreatePaymentPlansForAllNewAccounts();

        Task DeletePaymentPlansForCancelledAccounts();

        Task<IEnumerable<IAccount>> GetAccountsWaitingForPaymentPlan();

    }
}
