using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace MiseWebsite.Repositories
{
    public interface IResellerAccountRepository
    {
        Task<IEnumerable<IResellerAccount>> GetAll();
        Task<IEnumerable<IResellerAccount>> GetActiveResellers();

        Task<IEnumerable<IResellerAccount>> GetReseller(EmailAddress email, Password password);
    }
}
