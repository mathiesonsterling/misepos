using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Entities.Accounts
{
    public interface IBusinessAccount : IAccount
    {
        string BusinessName { get; }
    }
}
