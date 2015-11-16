using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Entities.Accounts;
using Mise.Core.Services.UtilityServices;
using stockboymobileserviceService.Services;
using stockboymobileserviceService.Services.Implementation;

namespace stockboymobileserviceService.ScheduledJobs
{
    public class CreateStripeSubsciptionsForNewAccountsJob : ScheduledJob
    {
        private readonly IAccountService _accountService;
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly ILogger _logger;
        public CreateStripeSubsciptionsForNewAccountsJob()
        {
            _accountService = new AccountService();
            _paymentProviderService = new StripePaymentProviderService();
            _logger = new DummyLogger();
        }

        public CreateStripeSubsciptionsForNewAccountsJob(IAccountService accountService, 
            IPaymentProviderService paymentProviderService, ILogger logger)
        {
            _accountService = accountService;
            _paymentProviderService = paymentProviderService;
            _logger = logger;
        }

        public async override Task ExecuteAsync()
        {
            var accounts = await _accountService.GetAccountsWaitingForPaymentPlan();
            if (accounts == null)
            {
                return;
            }

            var processed = new List<IAccount>();
            foreach (var account in accounts)
            {
                try
                {
                    await _paymentProviderService.CreateAccountFromToken(account.CurrentCard, account.PaymentPlan);

                    processed.Add(account);
                }
                catch (Exception e)
                {
                    _logger.HandleException(e);
                }
            }
        } 
    }
}
