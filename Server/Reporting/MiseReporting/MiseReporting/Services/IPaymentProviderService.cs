using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;

namespace MiseReporting.Services
{
    public interface IPaymentProviderService
    {
        Task CreateSubscriptionForAccount(IAccount account);

        Task CancelSubscriptionForAccount(IAccount account);
    }
}
