using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Core.Repositories
{
    public interface IAccountRepository : IEventSourcedEntityRepository<IAccount, IAccountEvent>
    {
        Task<IAccount> GetAccountForEmail(EmailAddress email);
        Task LoadAccount(Guid id);
    }
}
