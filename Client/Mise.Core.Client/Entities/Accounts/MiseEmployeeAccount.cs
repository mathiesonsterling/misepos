using Mise.Core.Entities.Accounts;

namespace Mise.Core.Client.Entities.Accounts
{
    public class MiseEmployeeAccount : BaseAccountEntity<IAccount, Common.Entities.Accounts.MiseEmployeeAccount>
    {
        public MiseEmployeeAccount() { }

        public MiseEmployeeAccount(Core.Common.Entities.Accounts.MiseEmployeeAccount source) : base(source)
        {
            
        }

        protected override Core.Common.Entities.Accounts.MiseEmployeeAccount CreateAccountSubobject()
        {
            return new Core.Common.Entities.Accounts.MiseEmployeeAccount();
        }
    }
}
