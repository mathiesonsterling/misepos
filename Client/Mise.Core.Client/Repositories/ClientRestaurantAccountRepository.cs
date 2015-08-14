using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Repositories
{
    public class ClientRestaurantAccountRepository : BaseEventSourcedClientRepository<IAccount, IAccountEvent, RestaurantAccount>, IAccountRepository
    {
        private IAccountWebService _webService;
        public ClientRestaurantAccountRepository(ILogger logger, IAccountWebService webService) : 
			base(logger, webService)
        {
            _webService = webService;
        }

        protected override IAccount CreateNewEntity()
        {
            return new RestaurantAccount();
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

        protected override Task<IEnumerable<RestaurantAccount>> LoadFromWebservice(Guid? restaurantID)
        {
            return Task.FromResult(new List<RestaurantAccount>().AsEnumerable());
        }
    }
}
