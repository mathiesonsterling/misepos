using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MiseReporting.Services;
using MiseReporting.Services.Implementation;

namespace MiseReporting.Controllers
{
    public class JobsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ISendReportsService _sendReportsService;
        public JobsController()
        {
            _accountService = new AccountService();
            _sendReportsService = new SendReportsService();
        }

        // GET: Jobs
        public async Task<string> CreatePaymentPlans()
        {
            try
            {
                await _accountService.CreatePaymentPlansForAllNewAccounts();
                await _accountService.DeletePaymentPlansForCancelledAccounts();
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

        public async Task SendCSVReports()
        {
            await _sendReportsService.SendCSVReportsForNewItems();
        }
    }
}
