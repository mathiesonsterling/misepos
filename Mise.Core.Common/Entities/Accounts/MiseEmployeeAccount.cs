using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    public class MiseEmployeeAccount : BaseAccount
    {
        public override void When(IAccountEvent entityEvent)
        {
            throw new NotImplementedException();
        }

        public override ICloneableEntity Clone()
        {
            throw new NotImplementedException();
        }

        public override MiseAccountTypes AccountType => MiseAccountTypes.MiseEmployee;

    }
}
