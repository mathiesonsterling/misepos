using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.Accounts
{
    public class MiseEmployeeAccount : RestaurantAccount
    {
        public override MiseAccountTypes AccountType
        {
            get { return MiseAccountTypes.MiseEmployee; }
        }

        protected override void WhenRegisteredFromMobile(AccountRegisteredFromMobileDeviceEvent ev)
        {
            throw new NotImplementedException("Cannot register EmployeeAccounts via events!");
        }

        public override IEnumerable<IAccountCharge> GetCharges()
        {
            return new List<IAccountCharge>();
        }

        public override IEnumerable<IAccountPayment> GetPayments()
        {
            return new List<IAccountPayment>();
        }
    }
}
