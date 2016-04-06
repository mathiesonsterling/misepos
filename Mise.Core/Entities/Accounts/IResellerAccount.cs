using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Entities.Accounts
{
    public interface IResellerAccount : IAccount
    {
        /// <summary>
        /// Who this person pushes sales up to.  Must be either head of Mise sales or MiseEmployee at the end of the chain
        /// </summary>
        Guid? ResellerUnderId { get; set; }
    }
}
