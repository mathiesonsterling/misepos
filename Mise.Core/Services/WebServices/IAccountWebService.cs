using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Accounts;

namespace Mise.Core.Services.WebServices
{
    public interface IAccountWebService : IEventStoreWebService<IAccount, IAccountEvent>
    {
    }
}
