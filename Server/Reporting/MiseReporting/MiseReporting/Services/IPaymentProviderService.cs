using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace MiseReporting.Services
{
    public interface IPaymentProviderService
    {
        Task CreateSubscriptionForAccount(IAccount account);
    }
}
