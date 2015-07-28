using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Common.Services;
using Mise.Core.Entities;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Check.Events.PaymentEvents.CreditCards;
using Mise.Core.Client.Repositories;
using Mise.Core.Client.Services;

namespace Mise.Core.Client.ApplicationModel.Implementation
{
    /// <summary>
    /// Class encapsulating all view behavior for a terminal application
    /// </summary>
    public class TerminalApplicationModel : ITerminalApplicationModel
    {
        public bool Setup { get; private set; }
        public bool Online { get; private set; }
        IUnitOfWork _unitOfWork;

        IRestaurantTerminalService _terminalService;
        readonly IMiseTerminalDevice _terminalSettings;
        private readonly IRestaurant _restaurant;
        ICheckRepository _checkRepository;
        IEmployeeRepository _employeeRepository;
        IMenuRepository _menuRepository;
        public Menu Menu { get; private set; }
        ILogger _logger;

        POSEventFactory _eventFactory;

        public MenuItemCategory TopCat { get; private set; }
        ICashDrawerService _cashDrawerService;

        /// <summary>
        /// Service to abstract out getting credit cards into the system
        /// </summary>
		public ICreditCardReaderService CreditCardReaderService{ get; private set;}
        ICreditCardProcessorService _creditCardProcessorService;
        ISalesTaxCalculatorService _salesTaxService;

        /// <summary>
        /// How we talk to the local printer, if there is one
        /// </summary>
        public IPrinterService LocalPrinterService { get; private set; }


        void DoSharedConstructorStuff(IUnitOfWork unitOfWork, IRestaurantTerminalService terminalWebService, ILogger logger,
            ICreditCardProcessorService creditCardProcessorService,
			ICashDrawerService cashDrawerService, ICreditCardReaderService creditCardReaderService,
            IPrinterService printerService,
            ISalesTaxCalculatorService salesTaxService,
            ICheckRepository checkRespository,
            IEmployeeRepository employeeRepos,
            IMenuRepository menuRepository
        )
        {
            _logger = logger;
            try
            {
                //get our dependencies for injection
                _checkRepository = checkRespository;
                _employeeRepository = employeeRepos;
                _menuRepository = menuRepository;

                _unitOfWork = unitOfWork;
                _unitOfWork.CheckRepository = _checkRepository;
                _unitOfWork.EmployeeRepository = _employeeRepository;


                _cashDrawerService = cashDrawerService;
                CreditCardReaderService = creditCardReaderService;
                _creditCardProcessorService = creditCardProcessorService;
                _salesTaxService = salesTaxService;
                _terminalService = terminalWebService;
                LocalPrinterService = printerService;

				//check our cc reader service matches our settings
				/*
				if((CreditCardReaderService.CreditCardReaderType & _terminalSettings.CreditCardReaderType) <= 0){
					//error
					throw new ArgumentException ("Credit card service of type " + CreditCardReaderService +" cant support requested type of " + _terminalSettings.CreditCardReaderType);
				}*/

                RequireEmployeeSignIn = _terminalSettings.RequireEmployeeSignIn;

                if (_terminalService == null)
                {
                    throw new NullReferenceException("TerminalService has not been set");
                }
                if (_restaurant == null)
                {
                    throw new NullReferenceException("Restaurant has not been set");
                }
                _eventFactory = new POSEventFactory(_terminalSettings, _restaurant);

                //get our menu
                Menu = _menuRepository.GetCurrentMenu();

                //get our employees 
                _employeeRepository = employeeRepos;

                //load our categories from the menu
                TopCat = _terminalSettings.TopLevelCategoryID.ToString() == Guid.Empty.ToString()
                    ? Menu.Categories.FirstOrDefault()
                    : Menu.Categories.FirstOrDefault(c => _terminalSettings.TopLevelCategoryID == c.ID);
                if (TopCat == null)
                {
                    throw new Exception("Cannot find category of type " + _terminalSettings.TopLevelCategoryID);
                }
                _allCategories = TopCat.SubCategories;

                //get all open checks

                ChecksLastUpdated = DateTime.Now;

                //set the cash drawer action
                //give the default
                if (_cashDrawerService != null)
                {
                    _cashDrawerService.DrawerClosed += (sender, e) =>
                    {
                        if (_cashDrawerAction == null) return;
                        CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
                        _cashDrawerAction();
                    };
                }

                Setup = true;
            }
            catch (Exception e)
            {
                _logger.HandleException(e);
				throw;
            }
        }

        /// <summary>
        /// Fully DI constructor for testing
        /// </summary>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="terminalWebService">Terminal web service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="cashDrawerService">Cash drawer service.</param>
        /// <param name="creditCardReaderService">Credit card hardware service.</param>
        /// <param name="creditCardProcessorService">Credit card processor service.</param>
        /// <param name="salesTaxService">Sales tax service.</param>
        /// <param name="printerService"></param>
        /// <param name="checkRespository">Check respository.</param>
        /// <param name="employeeRepos">Employee repos.</param>
        /// <param name="menuRepository">Menu repository.</param>
        /// <param name = "deviceSettings"></param>
        /// <param name="restaurant"></param>
        public TerminalApplicationModel(IUnitOfWork unitOfWork,
            IRestaurantTerminalService terminalWebService,
            ILogger logger,
                                 ICashDrawerService cashDrawerService,
                                 ICreditCardReaderService creditCardReaderService,
                                 ICreditCardProcessorService creditCardProcessorService,
                                 ISalesTaxCalculatorService salesTaxService,
            IPrinterService printerService,
            ICheckRepository checkRespository,
            IEmployeeRepository employeeRepos,
            IMenuRepository menuRepository,
            IMiseTerminalDevice deviceSettings,
            IRestaurant restaurant)
        {
            _terminalSettings = deviceSettings;
            _restaurant = restaurant;
            DoSharedConstructorStuff(unitOfWork, terminalWebService, logger, creditCardProcessorService, cashDrawerService,
                creditCardReaderService, printerService, salesTaxService, checkRespository, employeeRepos, menuRepository);
        }

        /// <summary>
        /// Regular client constructor
        /// </summary>
        /// <param name="unitOfWork">Unit of work.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="dal">Dal.</param>
        /// <param name="cashDrawerService">Cash drawer service.</param>
        /// <param name="creditCardReaderService">Credit card hardware service.</param>
        /// <param name="creditCardProcessorService">Credit card processor service.</param>
        /// <param name="printerService"></param>
        /// <param name="salesTaxService">Sales tax service.</param>
        /// <param name="terminalService"></param>
        public TerminalApplicationModel(IUnitOfWork unitOfWork, ILogger logger,
                                 IClientDAL dal,
                                 ICashDrawerService cashDrawerService,
                                 ICreditCardReaderService creditCardReaderService,
                                 ICreditCardProcessorService creditCardProcessorService,
                                 IPrinterService printerService,
                                 ISalesTaxCalculatorService salesTaxService,
                                 IRestaurantTerminalService terminalService
        )
        {

            //	#endif
            //init our terminal web service stuff here with our settings
            var terminalWebService = terminalService;
            try
            {
                /*
                var uri = new Uri("http://miserestaurantserver.azurewebsites.net/");
				var unkownRes = new Restaurant{
					FriendlyID = "dominieslic",
					RestaurantServerLocation = uri,
				};*/
                var regRes = terminalWebService.RegisterClientAsync("mainBar").Result;
                _terminalSettings = regRes.Item2;
                _restaurant = regRes.Item1;
                Online = true;
                logger.Log("Inserting restaurant from web service into database . . . .", LogLevel.Debug);
                //restaurantRepository.Update(new[] {_restaurant});

            }
            catch (Exception e)
            {
                _logger.HandleException(e);
                Online = false;
                //get our restaraunt stored
                IRestaurantRepository restaurantRepository = new ClientRestaurantRepository(logger, dal, terminalService, terminalWebService);
                _restaurant = restaurantRepository.GetAll().FirstOrDefault();
            }
				
            var checkRepos = new ClientCheckRepository(terminalWebService, dal, logger, terminalWebService);
            logger.Log("Loading check repository . . .", LogLevel.Debug);
            checkRepos.Load(_restaurant.ID);
            var employeeRepos = new ClientEmployeeRepository(terminalWebService, dal, logger, terminalWebService);
            logger.Log("Loading employee repository", LogLevel.Debug);
            employeeRepos.Load(_restaurant.ID);
            var menuRepos = new ClientMenuRepository(terminalWebService, dal, logger);
            logger.Log("Loading menu repository", LogLevel.Debug);
            menuRepos.Load();

            logger.Log("Repositories loaded, begining ViewModel construction", LogLevel.Debug);
            DoSharedConstructorStuff(unitOfWork, terminalWebService, logger, creditCardProcessorService, cashDrawerService, creditCardReaderService, printerService, salesTaxService
                , checkRepos, employeeRepos, menuRepos);
        }



        /// <summary>
        /// If true, the employee needs to put their passcode in to order
        /// </summary>
        public bool RequireEmployeeSignIn { get; private set; }

        public DateTime ChecksLastUpdated { get; private set; }

        public TerminalViewTypes CurrentTerminalViewTypeToDisplay { get; private set; }

        public bool Dirty
        {
            get
            {
                return _checkRepository.Dirty;
            }
        }

        #region Employees
        /// <summary>
        /// All available employees - might not be accessable for phones
        /// </summary>
        /// <value>The employees.</value>
        public IEnumerable<IEmployee> CurrentEmployees
        {
            get
            {
                return _employeeRepository.GetAll().Where(e => e.CurrentlyClockedInToPOS);
            }
        }

        /// <summary>
        /// The employee the application is currently dealing with, or null
        /// </summary>
        /// <value>The selected employee.</value>
        public IEmployee SelectedEmployee { get; set; }

        /// <summary>
        /// Stores the last selected employee for cross ordering
        /// </summary>
        /// <value>The last selected employee.</value>
        public IEmployee LastSelectedEmployee { get; set; }


        /// <summary>
        /// Fired when we click on an employee name
        /// </summary>
        /// <param name="employee"></param>
        public void EmployeeClicked(IEmployee employee)
        {
            //see if we're deselecting
            if (SelectedEmployee == employee)
            {
                SelectedEmployee = null;
                return;
            }


            SelectedEmployee = employee;
            LastSelectedEmployee = employee;
        }

        public bool EmployeeClockin(string passcode)
        {
            var thisEmp = _employeeRepository.GetByPasscode(passcode);

            if (thisEmp == null)
            {
                var failed = _eventFactory.CreateBadLoginEvent(passcode, "EmployeeClockin");
                _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { failed });
                return false;
            }

            //make an event here
            var clockinEvent = _eventFactory.CreateEmployeeClockedInEvent(thisEmp);
            SelectedEmployee = _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { clockinEvent });
            LastSelectedEmployee = SelectedEmployee;
			_unitOfWork.Commit (_terminalSettings.ID, SelectedEmployee, null);
            return true;
        }

        public bool EmployeeClockout(string passcode, IEmployee employee)
        {
            var thisEmp = _employeeRepository.GetByPasscode(passcode);

            if (thisEmp == null)
            {
                var failed = _eventFactory.CreateBadLoginEvent(passcode, "EmployeeClockout");
                _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { failed });
                return false;
            }

            //check we put the correct passcode in
            if (employee.Passcode != passcode) return false;

            //make an event here
            var clockoutEvent = _eventFactory.CreateEmployeeClockedOutEvent(thisEmp);
            SelectedEmployee = _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { clockoutEvent });

            if (thisEmp.ID == SelectedEmployee.ID)
            {
                SelectedEmployee = null;
            }

            if (LastSelectedEmployee != null && LastSelectedEmployee.ID == thisEmp.ID)
            {
                LastSelectedEmployee = null;
            }
            return true;
        }

        #endregion

        #region Checks
        public bool CanOpenChecks { get { return LastSelectedEmployee != null; } }

        public IEnumerable<ICheck> AllChecks
        {
            get
            {
                return _checkRepository.GetAll();
            }
        }

        /// <summary>
        /// All checks available
        /// </summary>
        /// <value>The checks.</value>
        public IEnumerable<ICheck> OpenChecks
        {
            get
            {
                return _checkRepository.GetOpenChecks(SelectedEmployee).OrderBy(c => c.DisplayName);
            }
        }

        public IEnumerable<ICheck> ClosedChecks
        {
            get
            {
                return _checkRepository.GetClosedChecks();
            }
        }

        /// <summary>
        /// The check we're currently working with in this application
        /// </summary>
        /// <value>The selected check that this terminal is currently dealing with.</value>
        public ICheck SelectedCheck { get; private set; }

        /// <summary>
        /// When a check is selected
        /// </summary>
        /// <param name="check"></param>
        public TerminalViewTypes CheckClicked(ICheck check)
        {
            try {
				//if we don't have a selected employee, select our last one
				if (SelectedEmployee == null) {
					SelectedEmployee = LastSelectedEmployee;
				}

				//we can only move forward if we have a selected employee!
				if (RequireEmployeeSignIn && SelectedEmployee == null) {
					return CurrentTerminalViewTypeToDisplay;
				}
				SelectedCheck = check;
				var startedTrans = _unitOfWork.Start (_terminalSettings.ID, SelectedEmployee, SelectedCheck);
				if (startedTrans == false) {
					_logger.Log ("Cannot start transaction");
					throw new InvalidOperationException ("Cannot start transaction!");
				}

				//change to view the check now, depending on if our check is closing or open
				switch (SelectedCheck.PaymentStatus) {
				case CheckPaymentStatus.Closing:
				case CheckPaymentStatus.PaymentRejected:
				case CheckPaymentStatus.Closed:
					CurrentTerminalViewTypeToDisplay = TerminalViewTypes.PaymentScreen;
					break;
				case CheckPaymentStatus.PaymentApprovedWithoutTip:
					CurrentTerminalViewTypeToDisplay = TerminalViewTypes.AddTips;
					break;
				default:
					CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;
					break;
				}
			}
            catch (Exception e)
            {
                _logger.HandleException(e);
            }

            return CurrentTerminalViewTypeToDisplay;
        }


        public ICheck CreateNewCheck(PersonName newName)
        {
            var ccev = _eventFactory.CreateCheckCreatedEvent(SelectedEmployee);
            var custEv = _eventFactory.CreateCustomerAssignedToTabEvent(ccev.CheckID, newName, SelectedEmployee);

            var events = new List<ICheckEvent> { ccev, custEv };
            var check = _checkRepository.ApplyEvents(events);
            if (check == null)
            {
                _logger.Log("Got back null tab from create!");
            }
            SelectedCheck = check;

            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;

            ChecksLastUpdated = DateTime.Now;
            return check;
        }

		public ICheck CreateNewCheck(CreditCard card)
		{
		    var ccev = _eventFactory.CreateCheckCreatedWithCreditCardEvent(SelectedEmployee, card);

            var custEv = _eventFactory.CreateCustomerAssignedToTabEvent(ccev.CheckID, card.Name, SelectedEmployee);

            var events = new List<ICheckEvent> { ccev, custEv };
            var check = _checkRepository.ApplyEvents(events);
            if (check == null)
            {
                _logger.Log("Got back null tab from create!");
            }
            SelectedCheck = check;

            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;

            ChecksLastUpdated = DateTime.Now;
            return check;
		}

        public bool CanModifyChecksServerDoesntOwn
        {
            get
            {
                return true;
            }
        }

        #region Payments

        public bool HasCashDrawer
        {
            get
            {
                return _terminalSettings.HasCashDrawer;
            }
        }

        public void CancelPayments()
        {
            if (SelectedCheck != null)
            {
                var cancelRes = _unitOfWork.Cancel(_terminalSettings.ID, SelectedEmployee, SelectedCheck);
				cancelRes.Wait ();
                SelectedCheck.PaymentStatus = CheckPaymentStatus.Closing;
                CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
                //reload our check, since it's changed back in the repository
                SelectedCheck = _checkRepository.GetByID(SelectedCheck.ID);
                SelectedEmployee = _employeeRepository.GetByID(SelectedEmployee.ID);
            }
        }

        public bool SavePaymentsClicked()
        {
            if (SelectedCheck != null)
            {
                var commitTask = _unitOfWork.Commit(_terminalSettings.ID, SelectedEmployee, SelectedCheck);
				commitTask.GetAwaiter ().GetResult ();
                CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
                return true;
            }
            return false;
        }

        public bool PayCheckWithCash(Money amountTendered)
        {
            if (SelectedEmployee == null)
            {
                throw new ArgumentException("Selected Employee is not set!");
            }

            var total = GetRemainingAmountToBePaidOnCheck();
            var finishes = amountTendered.GreaterThanOrEqualTo(total);

            var paid = amountTendered.GreaterThan(total) ? total : amountTendered;
            var change = finishes ? amountTendered.Subtract(total) : Money.None;
            //create our event
            var payEv = _eventFactory.CreateCashPaidOnCheckEvent(
                SelectedCheck,
                SelectedEmployee,
                amountTendered,
                paid,
                change);
            SelectedCheck = _checkRepository.ApplyEvent(payEv);

            return true;
        }


        public ICheck ReopenSelectedCheck()
        {
            var reEvent = _eventFactory.CreateReopenCheckEvent(SelectedCheck, SelectedEmployee);
            SelectedCheck = _checkRepository.ApplyEvent(reEvent);
            _unitOfWork.Commit(_terminalSettings.ID, SelectedEmployee, SelectedCheck);
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;
            return SelectedCheck;
        }

        public ICheck MarkSelectedCheckAsPaid()
        {
            if (SelectedCheck == null || SelectedEmployee == null)
            {
                _logger.Log("Attempting to close a check, but check or employee is null!", LogLevel.Debug);
                return null;
            }
            //check that current check has enough payments to be closed
            var remaining = GetRemainingAmountToBePaidOnCheck();
            if (remaining.HasValue)
            {
                var msg = "Attempting to close a check with a balance still due of amount " + remaining.ToFormattedString();
                _logger.Log(msg, LogLevel.Debug);
                throw new ArgumentException(msg);
            }

            //process any payments that haven't yet been sent
            var processPayments = SelectedCheck.GetPayments().OfType<IProcessingPayment>().Where(t => t.PaymentProcessingStatus == PaymentProcessingStatus.Created).ToList();
            if (processPayments.Any())
            {
                DoProcessPayments(processPayments);
            }

            //if it does, create a MarkAsPaid event
            var doneEvent = _eventFactory.CreateMarkCheckAsPaidEvent(SelectedCheck, SelectedEmployee);
            SelectedCheck = _checkRepository.ApplyEvent(doneEvent);

            if (SelectedCheck.PaymentStatus != CheckPaymentStatus.Closing)
            {
                //close us out!
                //TODO figure a way to get this to be aysnc!
                _unitOfWork.Commit(_terminalSettings.ID, SelectedEmployee, SelectedCheck);

                //determine if we're going to display change or not
                CurrentTerminalViewTypeToDisplay = SelectedCheck.GetPayments().Any(p => p.OpensCashDrawer)
                    ? TerminalViewTypes.DisplayChange
                    : TerminalViewTypes.ViewChecks;
            }
            //return our check
            return SelectedCheck;
        }


        Action<ICheck> _afterCreditCardProcessAction;
        public void SetCreditCardProcessedCallback(Action<ICheck> callback)
        {
            _afterCreditCardProcessAction = callback;
        }

        void DoProcessPayments(IEnumerable<IProcessingPayment> processPayments)
        {
            var ccPayments = processPayments.OfType<ICreditCardPayment>().ToList();
            //get our internal credit card payments!
            //TODO add submission events, so we can mark our stuff as in progress
            foreach (var payment in ccPayments)
            {
                var ccStart = _eventFactory.CreateCreditCardAuthorizationStartedEvent(SelectedCheck, SelectedEmployee,
                    payment);
                SelectedCheck = _checkRepository.ApplyEvent(ccStart);
            }


            _unitOfWork.DoWhenRepositoryIsAvailable(_terminalSettings.ID,
                SelectedEmployee, SelectedCheck, (emp, check) =>
                {
                    foreach (var ccPayment in ccPayments)
                    {
                        try
                        {
                            //submit to our service, and store as an event when done
                            _creditCardProcessorService.AuthorizeCard(ccPayment,
                                (payment, authCode) =>
                                {
                                    //now make our events, and apply them
                                    ICreditCardEvent authEvent;
                                    if (authCode.IsAuthorized)
                                    {
                                        authEvent = _eventFactory.CreateCreditCardAuthorizedEvent(
                                            check, emp, payment, authCode);
                                        //TODO add in call to print a receipt here, for each payment

                                    }
                                    else
                                    {
                                        authEvent = _eventFactory.CreateCreditCardFailedAuthorizationEvent(
                                            check, emp, payment, authCode);
                                    }
                                    //update the check selected if it still is - we might have moved on
                                    var res = _checkRepository.ApplyEvent(authEvent);
                                    if (SelectedCheck != null && SelectedCheck.ID == res.ID)
                                    {
                                        SelectedCheck = res;
                                    }

                                });//end authorize and callback
                        }
                        catch (Exception e)
                        {
                            _logger.HandleException(e);
                        }
                    }//end foreach payment

                    //if we were given a callback, we do it here
                    if (_afterCreditCardProcessAction != null)
                    {
                        try
                        {
                            _afterCreditCardProcessAction(check);
                        }
                        catch (Exception e)
                        {
                            _logger.HandleException(e);
                        }
                    }
                }); //end when repos available

        }

        public IEnumerable<Money> GetTypicalTipAmountsOnSelectedCheck(int numToGet)
        {
            var sourcePercentages = new List<decimal> { .15M, .2M, .25M, .10M, .33M, .40M };
            var total = SelectedCheck.Total;
            return sourcePercentages.Select(total.Multiply).Take(numToGet);
        }




        #endregion

        public Money GetSalesTaxForSelectedCheck()
        {
            return SelectedCheck == null ? null : _salesTaxService.CalculateSalesTax(SelectedCheck);

        }

        public Money GetTotalWithSalesTaxForSelectedCheck()
        {
            if (SelectedCheck == null)
            {
                return null;
            }

            var tax = _salesTaxService.CalculateSalesTax(SelectedCheck);
            return SelectedCheck.Total.Add(tax);
        }

        public Money GetRemainingAmountToBePaidOnCheck()
        {
            var amt = GetTotalWithSalesTaxForSelectedCheck();
            if (amt == null)
            {
                return null;
            }

            return SelectedCheck.GetPayments().Where(p => p.AmountToApplyToCheckTotal.HasValue)
                .Aggregate(amt, (current, payment) => current.Subtract(payment.AmountToApplyToCheckTotal));
        }

        #region Credit Cards
        public bool PayCheckWithCreditCard(CreditCard card, Money amt)
        {
            try
            {
                //we'll add the payment, then actually submit it once we close the payments
                //create the added events, which will add the transactions
                var ccPaymentEvent = _eventFactory.CreateCreditCardAddedForPaymentEvent(SelectedCheck, SelectedEmployee, amt, card);
                SelectedCheck = _checkRepository.ApplyEvent(ccPaymentEvent);

                return true;
            }
            catch (Exception e)
            {
                _logger.HandleException(e);
                return false;
            }
        }

        public IEnumerable<IProcessingPayment> WaitingForTipPayments
        {
            get
            {
                return SelectedCheck.GetPayments().OfType<IProcessingPayment>().Where(p => p.PaymentProcessingStatus == PaymentProcessingStatus.BaseAuthorized);
            }
        }

        public bool AddTipToProcessingPayment(IProcessingPayment payment, Money tipAmount)
        {
            if (payment == null)
            {
                _logger.Log("Payment was null");
                return false;
            }
            if (payment.PaymentProcessingStatus != PaymentProcessingStatus.BaseAuthorized)
            {
                _logger.Log("Payment was not authorized already!");
                return false;
            }

            var ccPayment = payment as ICreditCardPayment;
            if (ccPayment != null)
            {
                var tipEvent = _eventFactory.CreateCreditCardTipAddedToChargeEvent(SelectedCheck, SelectedEmployee, ccPayment, tipAmount);
                SelectedCheck = _checkRepository.ApplyEvent(tipEvent);

                //TODO this should check that we don't have any processing as well
                if (WaitingForTipPayments.Any() == false)
                {
                    if (SelectedCheck.GetPayments().OfType<IProcessingPayment>().All(p =>
                        p.PaymentProcessingStatus != PaymentProcessingStatus.SentForBaseAuthorization
                       || p.PaymentProcessingStatus == PaymentProcessingStatus.SentForFullAuthorization
                       ))
                    {
                        //amount is already good, so we're good to go
                        SelectedCheck.PaymentStatus = CheckPaymentStatus.Closed;

                        //if we're not waiting for Z's to process
                        if (_terminalSettings.WaitForZToCloseCards == false)
                        {
                            var closeReq = _eventFactory.CreateCreditCardCloseRequestedEvent(SelectedCheck, SelectedEmployee, ccPayment);
                            SelectedCheck = _checkRepository.ApplyEvent(closeReq);
                        }
                    }
                }

                //these should be sent to wait for Z instead!
                /*
                //send to processor service
                var selCheck = SelectedCheck;
                var selEmp = SelectedEmployee;
                _creditCardProcessorService.ChargeCard (ccPayment, ccPayment.AuthorizationResult, 
                    (retPayment, authCode) => {
                    //did we succeed?
                        var closeEvent = _eventFactory.CreateCreditCardCompletedEvent(selCheck, 
                            selEmp, retPayment, authCode, authCode.IsAuthorized);

                        var check = _checkRepository.ApplyEvent (closeEvent);

                        //we should fire back if we have a callback
                        //if we were given a callback, we do it here
                        if(_afterCreditCardProcessAction != null){
                            try{
                                _afterCreditCardProcessAction(check);
                            } catch(Exception e){
                                _logger.HandleException (e);
                            }
                        }
                });*/

                return true;
            }

            return false;
        }

        public bool VoidAuthorizedProcessingPayment(IProcessingPayment payment)
        {
            if (payment.PaymentProcessingStatus != PaymentProcessingStatus.BaseAuthorized)
            {
                return false;
            }

            var ccPayment = payment as ICreditCardPayment;
            var selCheck = SelectedCheck;
            var selEmployee = SelectedEmployee;
            if (ccPayment != null)
            {
                //TODO add request event?

                //send to processor service
                _creditCardProcessorService.VoidAuthorization(ccPayment, ccPayment.AuthorizationResult, (rPayment, authCode) =>
                {
                    //make the event
                    var reverseAuthEvent = _eventFactory.CreditCardAuthorizationCancelled(selCheck, selEmployee, rPayment, authCode);
                    var check = _checkRepository.ApplyEvent(reverseAuthEvent);

                    _unitOfWork.Commit(_terminalSettings.ID, selEmployee, selCheck);
                    //determine if we have other payments or not
                    CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;

                    if (_afterCreditCardProcessAction != null)
                    {
                        _afterCreditCardProcessAction(check);
                    }
                });
                return true;
            }
            return false;
        }

        public void AddExternalCreditCardPayment(Money amount, Money tipAmount)
        {
            throw new NotImplementedException();
        }
        #endregion


        public bool CompSelectedItem()
        {
            var compEvent = _eventFactory.CreateItemCompedEvent(SelectedCheck, SelectedEmployee, SelectedOrderItem,
                                string.Empty);

            SelectedCheck = _checkRepository.ApplyEvent(compEvent);
			SelectedEmployee = _employeeRepository.ApplyEvent (compEvent);
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;

            return true;
        }

		public void UndoCompOnSelectedOrderItem(){
			var uncompEvent = _eventFactory.CreateItemUncompedEvent (SelectedCheck, SelectedEmployee, SelectedOrderItem);
			SelectedCheck = _checkRepository.ApplyEvent (uncompEvent);
			SelectedEmployee = _employeeRepository.ApplyEvent (uncompEvent);
			CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;
		}

        public bool PayCheckWithAmountComp(Money amountToComp)
        {
            //create our event
            var payEv = _eventFactory.CreateCompPaidOnCheckEvent(SelectedCheck, SelectedEmployee, amountToComp);

            SelectedCheck = _checkRepository.ApplyEvent(payEv);

            //we also apply this to the employee!
            SelectedEmployee = _employeeRepository.ApplyEvent(payEv);
            //SelectedEmployee.CompAmount (amountToComp);

            return true;
        }
        #endregion

        #region Discounts
        public IEnumerable<IDiscount> GetPossibleDiscounts()
        {
            return _restaurant.GetPossibleDiscounts()
                .Where(d => d.AddsMoney == false)
                .Where(d => d.CanApplyToCheck(SelectedCheck));
        }

        public IEnumerable<IDiscount> GetPossibleGratuities()
        {
            return _restaurant.GetPossibleDiscounts()
                .Where(d => d.AddsMoney)
                .Where(d => d.CanApplyToCheck(SelectedCheck));
        }

        public ICheck AddDiscountsToSelectedCheck(IEnumerable<IDiscount> discounts)
        {
            List<DiscountAppliedToCheckEvent> discEvents = new List<DiscountAppliedToCheckEvent>();
            //make events, apply them
            foreach (var discount in discounts)
            {
                var disA = discount as DiscountAmount;
                if (disA != null)
                {
                    discEvents.Add(_eventFactory.CreateDiscountAppliedToCheckEvent(SelectedCheck, SelectedEmployee, disA));
                }
                else
                {
                    var disP = discount as DiscountPercentage;
                    if (disP != null)
                    {
                        discEvents.Add(_eventFactory.CreateDiscountAppliedToCheckEvent(SelectedCheck, SelectedEmployee,
                            disP));
                    }
                    else
                    {
                        var disAfter = discount as DiscountPercentageAfterMinimumCashTotal;
                        if (disAfter != null)
                        {
                            discEvents.Add(_eventFactory.CreateDiscountAppliedToCheckEvent(SelectedCheck,
                                SelectedEmployee, disAfter));
                        }
                    }
                }
            }
            SelectedCheck = _checkRepository.ApplyEvents(discEvents);

            return SelectedCheck;
        }

        public ICheck RemoveDiscountsFromSelectedCheck(IEnumerable<IDiscount> discounts)
        {
            var discEvents = discounts.Select(d => _eventFactory.CreateDiscountRemovedFromCheckEvent(SelectedCheck, SelectedEmployee, d));
            SelectedCheck = _checkRepository.ApplyEvents(discEvents);
            return SelectedCheck;
        }

        #endregion

        #region Drinks

        /// <summary>
        /// All categories we have
        /// </summary>
        IEnumerable<MenuItemCategory> _allCategories;
        public IEnumerable<MenuItemCategory> Categories { get { return _allCategories; } }

        public IEnumerable<MenuItemCategory> CurrentSubCategories
        {
            get
            {
                return SelectedCategory == null ? _allCategories : SelectedCategory.SubCategories;

            }
        }


        public MenuItemCategory GetCategoryByName(string name)
        {
            return GetCategoryByNameRecursive(TopCat, name);
        }

        MenuItemCategory GetCategoryByNameRecursive(MenuItemCategory startCat, string name)
        {
            if (startCat.Name == name)
            {
                return startCat;
            }

            return startCat.SubCategories
                .Select(subCats => GetCategoryByNameRecursive(subCats, name))
                .FirstOrDefault(found => found != null);
        }
        /// <summary>
        /// Given a menu item, find out which of our current categories it belongs to
        /// </summary>
        /// <returns>The currently displayed category for item.</returns>
        /// <param name="item">Item.</param>
        public MenuItemCategory GetCurrentlyDisplayedCategoryForItem(MenuItem item)
        {
            var cat = SelectedCategory ?? TopCat;

            //are we in the main?
            return cat.MenuItems.Any(m => m.ID == item.ID)
                ? SelectedCategory
                : cat.SubCategories.FirstOrDefault(subCat => MenuItemIsInCatRecursive(subCat, item));

        }

        bool MenuItemIsInCatRecursive(MenuItemCategory cat, MenuItem item)
        {
            return cat.MenuItems.Any(m => m.ID == item.ID) || cat.SubCategories.Any(subCat => MenuItemIsInCatRecursive(subCat, item));

        }


        /// <summary>
        /// Category we've currently selected
        /// </summary>
        /// <value>The current category.</value>
        public MenuItemCategory SelectedCategory { get; set; }

        public MenuItemCategory SelectedCategoryParent
        {
            get
            {
                if (SelectedCategory == null)
                {
                    return null;
                }
                return TopCat.SubCategories.Any(c => c.ID == SelectedCategory.ID)
                    ? null
                    : GetCategoryParentRecursively(TopCat, SelectedCategory.ID);

            }
        }

        static MenuItemCategory GetCategoryParentRecursively(MenuItemCategory current, Guid childCatID)
        {
            if (current.SubCategories.Any(c => c.ID == childCatID))
            {
                return current;
            }

            return current.SubCategories.Select(subCat => GetCategoryParentRecursively(subCat, childCatID))
                .FirstOrDefault(found => found != null);
        }

        public IEnumerable<MenuItem> MiseItems
        {
            get
            {
                //TODO - if the employee has some, take those!
                //else default
                return Menu.DefaultMiseItems;
            }
        }

        /// <summary>
        /// All menu items under this category and any of its subcategories
        /// </summary>
        /// <value>The menu items under current category.</value>
        public IEnumerable<MenuItem> MenuItemsUnderCurrentCategory
        {
            get
            {
                //recurse throught this category and all others to get the items
                if (SelectedCategory != null)
                {
                    return GetMenuItemsInCategoryRecursively(SelectedCategory).OrderBy(m => m.ButtonName);
                }
                return MiseItems;
            }
        }

        [Obsolete("Done in ViewModel now!")]
        public IEnumerable<Tuple<MenuItemCategory, IEnumerable<MenuItem>>> GetCategoryNamesAndMenuItems(int maxItemsOnScreen, int? maxItemsInRow)
        {
            if (SelectedCategory == null)
            {
                return new List<Tuple<MenuItemCategory, IEnumerable<MenuItem>>>{
					new Tuple<MenuItemCategory, IEnumerable<MenuItem>>(null, MiseItems.Take(maxItemsOnScreen))
				};
            }
            //our category is selected, so we want to get our menu items, then each category
            var list = new List<Tuple<MenuItemCategory, IEnumerable<MenuItem>>>
		    {
		        new Tuple<MenuItemCategory, IEnumerable<MenuItem>>(SelectedCategory, SelectedCategory.MenuItems)
		    };

            list.AddRange(maxItemsInRow.HasValue
                ? SelectedCategory.SubCategories.Select(subCat =>
                        new Tuple<MenuItemCategory, IEnumerable<MenuItem>>(subCat,
                            subCat.MenuItems.Take(maxItemsInRow.Value)))
                : SelectedCategory.SubCategories.Select(
                    subCat => new Tuple<MenuItemCategory, IEnumerable<MenuItem>>(subCat, subCat.MenuItems)));


            return list;
        }

        public IEnumerable<MenuItem> HotItemsUnderCurrentCategory
        {
            get
            {
                if (SelectedCheck == null)
                {
                    return new List<MenuItem>();
                }

                //if we have a category, get the most popular there
				if(SelectedCategory != null){
					return MenuItemsUnderCurrentCategory.OrderByDescending (m => m.DisplayWeight).ThenBy (m => m.ButtonName).ToList ();
				}

                //do we have items on the check?
                var itemsWithoutModsOnCheck = SelectedCheck.OrderItems
                    .Where (oi => oi.Modifiers.Any () == false).ToList();
                if (false == itemsWithoutModsOnCheck.Any())
                    return GetMenuItemsInCategoryRecursively(TopCat).OrderByDescending(mi => mi.DisplayWeight);
                var uniqueItems = new Dictionary<Guid, MenuItem> ();

                foreach (var oi in itemsWithoutModsOnCheck
                    .Where(oi => uniqueItems.ContainsKey(oi.MenuItem.ID) == false)
                    .OrderByDescending (o => o.CreatedDate))
                {
					if (uniqueItems.ContainsKey (oi.MenuItem.ID) == false) {
						uniqueItems.Add (oi.MenuItem.ID, oi.MenuItem);
					}
                }
                return uniqueItems.Values;
            }
        }

        static IEnumerable<MenuItem> GetMenuItemsInCategoryRecursively(MenuItemCategory category)
        {
            var items = new List<MenuItem>();
            items.AddRange(category.MenuItems);
            foreach (var subCat in category.SubCategories)
            {
                items.AddRange(GetMenuItemsInCategoryRecursively(subCat));
            }

            return items;
        }



        /// <summary>
        /// Orders a drink when clicked
        /// </summary>
        /// <param name="drink">Drink.</param>
        public OrderItem DrinkClicked(MenuItem drink)
        {
            //make an event to create the order item
            var orderItem = new OrderItem
            {
                CreatedDate = DateTime.UtcNow,
                MenuItem = drink,
                PlacedByID = SelectedEmployee.ID,
                ID = Guid.NewGuid(),
            };
            if (drink.Destinations != null)
            {
                foreach (var dest in drink.Destinations)
                {
                    orderItem.AddDestination(dest);
                }
            }
            var events = new List<ICheckEvent>{
				_eventFactory.CreateOrderedOnCheckEvent (orderItem, SelectedEmployee, SelectedCheck)
			};

			var defaultMods = drink.GetDefaultModifiers ().ToList ();
			if (defaultMods.Any())
            {
                events.Add(
					_eventFactory.CreateOrderItemModifiedEvent(orderItem, defaultMods, SelectedEmployee, SelectedCheck)
                );
            }

            SelectedCheck = _checkRepository.ApplyEvents(events);

            //TODO move this check to the view model, etc
			if (orderItem.NeedsModifiers) {
				CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ModifyOrder;
				SelectOrderItemToModify (orderItem);
			} else {
				CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;
			}

            return orderItem;
        }

        /// <summary>
        /// Orders the item ordering completed.
        /// </summary>
        /// <param name="item">Item.</param>
        public void OrderItemOrderingCompleted(OrderItem item)
        {
            item.Status = OrderItemStatus.Added;
        }

        public MenuItemCategory GetCategoryForItem(MenuItem menuItem)
        {
            return (from c in Categories
                    let miNames = c.MenuItems.Select(mi => mi.Name)
                    where miNames.Contains(menuItem.Name)
                    select c).FirstOrDefault();
        }

        #endregion


        #region Ordered Drinks
        //drinks are under the selected category

        public OrderItem SelectedOrderItem { get; private set; }
        /// <summary>
        /// When a drink that is already ordered is clicked
        /// </summary>
        /// <param name="drink">Drink.</param>
        public void OrderedDrinkClicked(OrderItem drink)
        {
        }

        /// <summary>
        /// Go to our home screen
        /// </summary>
        public void GoToHomeScreen()
        {
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
        }

        #endregion

        #region Modifiers

        public bool DeleteSelectedOrderItem()
        {
            var remEvent = _eventFactory.CreateOrderItemDeletedEvent(SelectedCheck,
                                                                      SelectedOrderItem,
                                                                      SelectedEmployee);

            SelectedCheck = _checkRepository.ApplyEvent(remEvent);

            return RemoveSelectedOrderItem();
        }

        bool RemoveSelectedOrderItem()
        {
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;
            if (SelectedOrderItem != null)
            {
                SelectedCheck.RemoveOrderItem(SelectedOrderItem);
                SelectedOrderItem = null;
                return true;
            }

            return false;
        }

        public bool VoidSelectedOrderItem(string managerPasscode, string reason, bool doWaste)
        {
            if (SelectedOrderItem != null)
            {
                var currStatus = SelectedOrderItem.Status;

                //check manager is valid
                var manager = _employeeRepository.GetAll().FirstOrDefault(e => e.Passcode == managerPasscode);
                if (manager == null)
                {
                    var failEv = _eventFactory.CreateBadLoginEvent(managerPasscode, "void");
                    _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { failEv });
                    return false;
                }

                //check we have permission
                if (manager.CanVoid(currStatus) == false)
                {
                    var permEV = _eventFactory.CreateInsufficientPermissionEvent(manager, "void");
                    _employeeRepository.ApplyEvents(new[] { permEV });
                    return false;
                }

                //pull the server ID and status we currently have
                //create event, put it into checks repos
                ICheckEvent voidEvent;
                if (doWaste)
                {
                    voidEvent = _eventFactory.CreateOrderItemWastedEvent(SelectedCheck, SelectedEmployee, manager,
                        SelectedOrderItem, reason);
                }
                else
                {
                    voidEvent = _eventFactory.CreateOrderItemVoidedEvent(SelectedCheck, SelectedEmployee, manager,
                        SelectedOrderItem, reason);
                }
                SelectedCheck = _checkRepository.ApplyEvent(voidEvent);

                //send the notification to all 
                foreach (var dest in SelectedOrderItem.Destinations)
                {
                    _terminalService.NotifyDestinationOfVoid(dest, SelectedOrderItem);
                }

                //remove item
                return RemoveSelectedOrderItem();
            }
            return false;
        }

        public OrderItem ReorderSelectedOrderItem()
        {
            if (SelectedOrderItem == null)
            {
                return null;
            }

            //get the mods
            var mods = SelectedOrderItem.Modifiers.ToList();

            var oi = DrinkClicked(SelectedOrderItem.MenuItem);
            if (mods.Any())
            {
                SelectedOrderItem = oi;
                ModifySelectedOrderItem(mods);
            }

            SelectedOrderItem = null;
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;

            oi.Status = OrderItemStatus.Added;
            return oi;
        }

        public void SelectOrderItemToModify(OrderItem oi)
        {
            SelectedOrderItem = oi;
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ModifyOrder;
        }

        public bool ModifySelectedOrderItem(IList<MenuItemModifier> modifiers)
        {
            var eventsList = new List<ICheckEvent>();
            if (SelectedOrderItem != null && SelectedOrderItem.MenuItem != null)
            {
                //see if all our required modifiers are set
                var reqModifiers = SelectedOrderItem.MenuItem.PossibleModifiers.Where(m => m.Required);

                if (reqModifiers.Select(reqModifier => reqModifier.Modifiers.Select(m => m.Name).ToList()).Select(modsForThisGroup => modifiers.Any(givenMod => modsForThisGroup.Contains(givenMod.Name))).Any(foundMod => foundMod == false))
                {
                    return false;
                }

                if (modifiers != null)
                {
                    //send it to our repository
                    var modEvent = _eventFactory.CreateOrderItemModifiedEvent(SelectedOrderItem, modifiers,
                                                                           SelectedEmployee, SelectedCheck);
                    eventsList.Add(modEvent);
                }
            }

            SelectedCheck = _checkRepository.ApplyEvents(eventsList);
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;

            return true;
        }

        public Money CalculateNewRunningPriceForMod(MenuItem item, IEnumerable<MenuItemModifier> mods)
        {
            return OrderItem.GetModifiedTotal(item, mods.ToList());
        }

        public bool SetMemoOnSelectedOrderItem(string memo)
        {
            if (string.IsNullOrEmpty(memo) == false)
            {
                var newEvent = _eventFactory.CreateOrderItemSetMemoEvent(SelectedCheck, SelectedOrderItem,
                                                                         memo, SelectedEmployee);
                SelectedCheck = _checkRepository.ApplyEvent(newEvent);
                return true;
            }
            return false;
        }


        public void CancelModificationOnSelectedOrderItem()
        {
            if (SelectedCheck != null && SelectedOrderItem != null)
            {
                if (SelectedCheck.OrderItems.Contains(SelectedOrderItem))
                {
                    if (SelectedOrderItem.Status == OrderItemStatus.Ordering)
                    {
                        SelectedCheck.RemoveOrderItem(SelectedOrderItem);
                    }
                }

                SelectedOrderItem = null;
            }
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.OrderOnCheck;
        }

        #endregion

        public TerminalViewTypes CloseOrderButtonClicked()
        {
            SendSelectedCheck();

            if (SelectedCheck != null
               && SelectedCheck.Total != null
               && SelectedCheck.Total.HasValue)
            {
                //mark our current tab as paying, send it to the cash screen
                SelectedCheck.PaymentStatus = CheckPaymentStatus.Closing;
                CurrentTerminalViewTypeToDisplay = TerminalViewTypes.PaymentScreen;
            }
            else
            {
                //TODO add an event here
                SelectedCheck.PaymentStatus = CheckPaymentStatus.Closed;
                CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
            }

            return CurrentTerminalViewTypeToDisplay;
        }

        public bool UseIntegratedCredit
        {
            get
            {
				return _terminalSettings.CreditCardReaderType > CreditCardReaderType.External;
            }
        }

        public void SendSelectedCheck()
        {
            //create a send event, and fire it to update
            var sendEvent = _eventFactory.CreateCheckSentEvent(SelectedCheck, SelectedEmployee);
			var check = _checkRepository.ApplyEvent(sendEvent);
            if (check != null)
            {
                SelectedCheck = check;
				var commitTask = _unitOfWork.Commit (_terminalSettings.ID, SelectedEmployee, SelectedCheck)
					.ContinueWith (task => {

					//destinations will be taken care of on restaurant server
					if (_terminalSettings.PrintKitchenDupes && LocalPrinterService != null) {
						var printTask = LocalPrinterService.PrintDupeAsync (SelectedCheck);
							printTask.Wait();
						}


					//move back to tab view
					CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
				});

				commitTask.Wait ();
            }
        }

        public Task SendSelectedCheckAsync()
        {
            //create a send event, and fire it to update
            var sendEvent = _eventFactory.CreateCheckSentEvent(SelectedCheck, SelectedEmployee);
			var check = _checkRepository.ApplyEvent(sendEvent);
            if (check != null)
            {
                SelectedCheck = check;
				var commitTask = _unitOfWork.Commit (_terminalSettings.ID, SelectedEmployee, SelectedCheck);

                //destinations will be taken care of on restaurant server
                if (_terminalSettings.PrintKitchenDupes && LocalPrinterService != null)
                {
                    LocalPrinterService.PrintDupeAsync(SelectedCheck);
                }

                //move back to check view
                CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
				return commitTask;
            }
            throw new ArgumentException("Error applying sent check event to check " + SelectedCheck.ID);
        }

        /// <summary>
        /// Undo any changes since our last saving
        /// </summary>
        public void CancelOrdering()
        {
            if (SelectedCheck != null)
            {
                _unitOfWork.Cancel(_terminalSettings.ID, SelectedEmployee, SelectedCheck);
                if (SelectedEmployee != null)
                {
                    SelectedEmployee = _employeeRepository.GetByID(SelectedEmployee.ID);
                }
            }
            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.ViewChecks;
            SelectedCheck = null;
			SelectedCategory = null;
        }

        #region CashDrawer
        private Action _cashDrawerAction;
        public void SetCashDrawerClosedEvent(Action toDoWhenClosed)
        {
            _cashDrawerAction = toDoWhenClosed;
        }

        public void OpenCashDrawer()
        {
            if (_cashDrawerService != null)
            {
                _cashDrawerService.OpenDrawer();
            }
        }

        public bool NoSale(string passcode)
        {
            var thisEmp = _employeeRepository.GetByPasscode(passcode);

            if (thisEmp == null)
            {
                var failed = _eventFactory.CreateBadLoginEvent(passcode, "NoSale");
                _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { failed });
                return false;
            }

            //make an event for our no sale
            var noSaleEvent = _eventFactory.CreateNoSaleEvent(thisEmp);
            _employeeRepository.ApplyEvents(new List<IEmployeeEvent> { noSaleEvent });

            CurrentTerminalViewTypeToDisplay = TerminalViewTypes.NoSale;
            return true;
        }
        #endregion
    }
}
