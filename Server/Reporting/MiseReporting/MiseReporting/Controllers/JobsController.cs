using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MiseReporting.Services;
using MiseReporting.Services.Implementation;

namespace MiseReporting.Controllers
{
    public class JobsController : Controller
    {
        private readonly IAccountService _accountService;
        public JobsController()
        {
            _accountService = new AccountService();
        }
        // GET: Jobs
        public async Task<string> CreatePaymentPlans()
        {
            try
            {
                await _accountService.CreatePaymentPlansForAllNewAccounts();
                return "Finished";
            }
            catch (AggregateException ag)
            {
                var msg = ag.InnerExceptions.Aggregate("Errors creating subscriptions ", (current, ex) => current + ("<br/>" + ex.Message));
                return msg;
            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }
        }

    }
}
