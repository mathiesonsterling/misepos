using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Repositories
{
    public class ClientRestaurantAccountRepository : BaseEventSourcedClientRepository<IAccount, IAccountEvent>, IAccountRepository
    {
        private IAccountWebService _webService;
        public ClientRestaurantAccountRepository(ILogger logger, IClientDAL dal, IAccountWebService webService) : base(logger, dal, webService)
        {
            _webService = webService;
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

        public Task<IAccount> GetAccountForEmail(EmailAddress email)
        {
            var acct = GetAll().FirstOrDefault(a => a.PrimaryEmail.Equals(email));
            return Task.FromResult(acct);
        }

        public override Task Load(Guid? restaurantID)
        {
            //we don't load anything here!  we only go outward on the client for now
            return Task.FromResult(false);
        }
    }
}
