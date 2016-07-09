using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    public class ResellerAccount : BaseAccount, IResellerAccount
    {
        public override void When(IAccountEvent entityEvent)
        {
            throw new NotImplementedException();
        }

        public override ICloneableEntity Clone()
        {
            throw new NotImplementedException();
        }

        public override MiseAccountTypes AccountType  => MiseAccountTypes.Reseller;

        public Guid? ResellerUnderId { get; set; }
        public bool IsActive { get; set; }
    }
}
