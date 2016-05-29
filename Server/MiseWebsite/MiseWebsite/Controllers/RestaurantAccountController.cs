using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using MiseWebsite.Database;
using MiseWebsite.Database.Implementation;
using MiseWebsite.Models;
using MiseWebsite.Services.Implementation;

namespace MiseWebsite.Controllers
{
    public class RestaurantAccountController : Controller
    {
        private readonly IAccountDAL _accountDAL;
        private readonly ICreditCardProcessorService _creditCardProcessorService;
        public RestaurantAccountController(IAccountDAL accountDAL, ICreditCardProcessorService creditCardProcessor)
        {
            _accountDAL = accountDAL;
            _creditCardProcessorService = creditCardProcessor;
        }

        public RestaurantAccountController() : this(new AccountDAL(), null)
        {
            var logger = new DummyLogger();
            IClientStripeFacade stripeClientFacade = new WebsiteStripeProcessor();

            _creditCardProcessorService = new StripePaymentProcessorService(logger, stripeClientFacade);
        }

        // GET: RestaurantAccount
        public ActionResult Index()
        {
            return View();
        }

        // GET: RestaurantAccount/Details/5
        public ActionResult Details(Guid id)
        {
            return View();
        }

        // GET: RestaurantAccount/Create
        public ActionResult Create()
        {
            //check if this email already is an employee

            //if we have a current user, prepopulate
            var vm = new RestaurantAccountViewModel();
            if (!string.IsNullOrEmpty(User.Identity?.Name))
            {
                vm.EmailAddress = User.Identity.Name;
                vm.ExpMonth = DateTime.Now.Month;
                vm.ExpYear = DateTime.Now.Year;
            }
            return View(vm);
        }

        // POST: RestaurantAccount/Create
        [HttpPost]
        public async Task<ActionResult> Create(RestaurantAccountViewModel viewModel)
        {
            try
            {
                //setup billing
                var card = new CreditCardNumber
                {
                    ExpYear = viewModel.ExpYear,
                    ExpMonth = viewModel.ExpMonth,
                    BillingZip = new ZipCode(viewModel.BillingZip),
                    CVC = viewModel.CVC,
                    Number = viewModel.CardNumber
                };

                var cardholder = new PersonName(viewModel.CardholderFirstName, viewModel.CardholderLastName);
                var cardRes = await _creditCardProcessorService.SendCardToProcessorForSubscription(cardholder, card);

                if (cardRes != null)
                {
                    //create account
                    var entity = new RestaurantAccount();

                    var addRes = await _accountDAL.AddRestaurantAccount(entity);

                    return RedirectToAction("IndexForUser", "Restaurant");
                }
                return View(viewModel);
            }
            catch(Exception e)
            {
                return View(viewModel);
            }
        }

        // GET: RestaurantAccount/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RestaurantAccount/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RestaurantAccount/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RestaurantAccount/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
