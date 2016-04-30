using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace MiseWebsite.Database
{
    public interface IAccountDAL
    {
        Task<IEnumerable<IResellerAccount>> GetActiveResellerAccounts();

        Task<IEnumerable<IResellerAccount>> GetResellerAccounts(EmailAddress email);

        Task<IResellerAccount> GetResellerAccount(EmailAddress email, Password pwd);
        
        Task<IResellerAccount> GetResellerAccount(Guid id);

        Task<IResellerAccount> AddResellerAccount(IResellerAccount acct);
        Task<IResellerAccount> UpdateResellerAccount(IResellerAccount acct);

        Task<IBusinessAccount> AddRestaurantAccount(RestaurantAccount restaurantAccount);
    }
}
