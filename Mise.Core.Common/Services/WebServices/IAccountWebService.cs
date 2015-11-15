using System;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities.Accounts;
namespace Mise.Core.Common.Services.WebServices
{
    public interface IAccountWebService : IEventStoreWebService<RestaurantAccount, IAccountEvent>
    {
        Task<IAccount> GetAccountById(Guid id);
        Task<IAccount> GetAccountFromEmail(EmailAddress email);
    }
}
