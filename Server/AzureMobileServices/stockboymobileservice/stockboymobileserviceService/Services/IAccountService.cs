using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;

namespace stockboymobileserviceService.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<IAccount>> GetAccountsWaitingForPaymentPlan();

        Task MarkAccountsAsHavingPaymentPlan(IEnumerable<IAccount> accounts);
    }
}
