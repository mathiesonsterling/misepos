using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Events;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Services.UtilityServices;

namespace MiseReporting.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly ManagementDAL _dal;
        private readonly IInventoryAppEventFactory _eventFactory;
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly ILogger _logger;
        public AccountService()
        {
            _dal = new ManagementDAL();
            _eventFactory = new InventoryAppEventFactory("MobileServices", MiseAppTypes.MobileService);
            _paymentProviderService = new StripePaymentProviderService();
            _logger = new DummyLogger();
        }


        public async Task CreatePaymentPlansForAllNewAccounts()
        {
            var accounts = await GetAccountsWaitingForPaymentPlan();

            var exceptions = new List<Tuple<IAccount, Exception>>();
            foreach (var account in accounts)
            {
                try
                {
                    await _paymentProviderService.CreateSubscriptionForAccount(account);

                    await MarkAccountAsHavingPaymentPlan(account);
                }
                catch (Exception e)
                {
                    _logger.HandleException(e);
                    exceptions.Add(new Tuple<IAccount, Exception>(account, e));
                }
            }

            if (exceptions.Any())
            {
                var msg = "Error creating subscriptions for " + exceptions.Count() + " accounts";
                var agg = new AggregateException(msg, exceptions.Select(t => t.Item2));
                throw agg;
            }
        }

        public async Task<IEnumerable<IAccount>> GetAccountsWaitingForPaymentPlan()
        {
            const string MISSING_ACCOUNTS_TAG = "\"PaymentPlanSetupWithProvider\":false";

            var accts = await _dal.GetRestaurantAccounts(MISSING_ACCOUNTS_TAG);
            return accts;
        }

        private async Task MarkAccountAsHavingPaymentPlan(IAccount account)
        {
            var ev = _eventFactory.CreateAccountHasPaymentPlanSetupEvent(account);
            account.When(ev);

            //todo - save the event into our DB as well

            //save the change!
            await _dal.UpdateAccount(account);
        }

    }
}
