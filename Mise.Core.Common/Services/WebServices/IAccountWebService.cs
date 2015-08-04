using Mise.Core.Entities.Accounts;
using Mise.Core.Common.Entities.Accounts;
namespace Mise.Core.Common.Services.WebServices
{
    public interface IAccountWebService : IEventStoreWebService<RestaurantAccount, IAccountEvent>
    {
    }
}
