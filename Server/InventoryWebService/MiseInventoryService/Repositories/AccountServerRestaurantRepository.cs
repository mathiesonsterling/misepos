using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class AccountServerRestaurantRepository : BaseAdminServiceRepository<IAccount, IAccountEvent>, IAccountRepository
    {
        public AccountServerRestaurantRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment hostingEnvironment) : base(logger, entityDAL, hostingEnvironment)
        {
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.AddAccountAsync, EntityDAL.UpdateAccountAysnc);
        }

        public Task<IAccount> GetAccountForEmail(EmailAddress email)
        {
            var account = GetAll().FirstOrDefault(a => a.PrimaryEmail.Equals(email));
            return Task.FromResult(account);
        }

        protected override IAccount CreateNewEntity()
        {
            return new RestaurantAccount();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is AccountRegisteredFromMobileDeviceEvent;
        }

        public override Guid GetEntityID(IAccountEvent ev)
        {
            return ev.AccountID;
        }

        protected override async Task LoadFromDB()
        {
            var items = await EntityDAL.GetAccountsAsync();
            Cache.UpdateCache(items);
        }
    }
}
