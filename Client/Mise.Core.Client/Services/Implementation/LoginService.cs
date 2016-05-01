using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.Common.Events;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Client.Services;
using Mise.Core.Common.Entities;
namespace Mise.Core.Client.Services.Implementation
{
    public class LoginService : ILoginService
	{
		readonly IEmployeeRepository _employeeRepository;
		readonly IInventoryAppEventFactory _eventFactory;
		readonly IRestaurantRepository _restaurantRepository;
		readonly IApplicationInvitationRepository _inviteRepository;
		readonly IAccountRepository _accountRepository;
		readonly IClientKeyValueStorage _keyValStorage;
		readonly ILogger _logger;
	    private readonly IRepositoryLoader _repositoryLoader;
		private readonly ICreditCardProcessorService _ccProcessor;
		IEmployee _currentEmployee;
		IRestaurant _currentRestaurant;

		public class MiseLoginRecord{
			public EmailAddress Email{get;set;}
			public string Hash{get;set;}
			public DateTime Time{get;set;}
		}

		public class RestaurantSelectRecord
		{
			public Guid RestaurantID{ get; set;}
		}

		public LoginService(IEmployeeRepository employeeRepository,
							IRestaurantRepository restaurantRepository,
							IApplicationInvitationRepository inviteRepository,
							IAccountRepository accountRepository,
		                    IInventoryAppEventFactory eventFactory,
							IClientKeyValueStorage keyValStorage,
		                    ILogger logger,
            				IRepositoryLoader repositoryLoader,
							ICreditCardProcessorService ccService
            )
		{
			_employeeRepository = employeeRepository;
			_restaurantRepository = restaurantRepository;
			_inviteRepository = inviteRepository;
			_eventFactory = eventFactory;
			_accountRepository = accountRepository;
			_keyValStorage = keyValStorage;
			_logger = logger;
		    _repositoryLoader = repositoryLoader;
			_ccProcessor = ccService;
		}

		const string LOGGED_IN_EMPLOYEE_KEY = "LoggedInEmployee";
		const string LAST_RESTAURANT_ID_KEY = "LastRestaurantID";

		public async Task<bool> LoadSavedEmployee(){
			bool needsDelete = false;
			try{
				//if we have an employee that logged in, less than 7 days ago, then mark it
				var login = _keyValStorage.GetValue<MiseLoginRecord> (LOGGED_IN_EMPLOYEE_KEY);
				if(login != null){
					return await LoadLoggedInEmployee(login);
				}
			} catch(Exception e){
				needsDelete = true;
				_logger.HandleException (e, LogLevel.Warn);
			}

			if (needsDelete) {
				try{
					await _keyValStorage.DeleteValue(LOGGED_IN_EMPLOYEE_KEY);
				} catch(Exception e){
					_logger.HandleException (e);
				}
			}

			return false;
		}

		private async Task<bool> LoadLoggedInEmployee(MiseLoginRecord login){
			if (login.Time > DateTime.UtcNow.AddDays (-7)) {
				var password = new Password{ HashValue = login.Hash };
				try {
					_currentEmployee = await _employeeRepository.GetByEmailAndPassword (login.Email, password);

					//we need to also get our restaurants - if there's only one, pick that!
					var rests = (await GetPossibleRestaurantsForLoggedInEmployee()).ToList();
					if(rests.Count() == 1){
						await SelectRestaurantForLoggedInEmployee(rests.First().Id);
						await LoadSelectedRestaurant();
					} else {
						//did we store the last restuarant?
						var lastRestRecord = _keyValStorage.GetValue<RestaurantSelectRecord>(LAST_RESTAURANT_ID_KEY);
						if(lastRestRecord != null){
							var rest = _restaurantRepository.GetByID(lastRestRecord.RestaurantID);
							if(rest != null){
								await SelectRestaurantForLoggedInEmployee(rest.Id);
								await LoadSelectedRestaurant();
							} else {
								_currentEmployee = null;
							}
						} else{
							_currentEmployee = null;
						}
					}
				} catch (Exception e) {
					_logger.HandleException (e);
				}
			}

			return _currentEmployee != null;
		}

		/// <summary>
		/// Sets the current employee.  Used only for testing!
		/// </summary>
		/// <param name="emp">Emp.</param>
		public void SetCurrentEmployee(IEmployee emp){
			_currentEmployee = emp;
		}

		/// <summary>
		/// Used only for debugging!
		/// </summary>
		/// <param name="rest">Rest.</param>
		public void SetCurrentRestaurant(IRestaurant rest){
			_currentRestaurant = rest;
		}

		public async Task<IEmployee> LoginAsync(EmailAddress email, Password password)
		{
			var emp = await _employeeRepository.GetByEmailAndPassword (email, password);

			if(emp == null){
				return null;
			}
			var loginEvent = _eventFactory.CreateEmployeeLoggedIntoInventoryAppEvent(emp);
			_employeeRepository.ApplyEvent(loginEvent);
			await _employeeRepository.Commit(emp.Id);

			_currentEmployee = emp;

            //update the application invitation repository with this email!
		    await _inviteRepository.Load(email);

			try{
				var loginRec = new MiseLoginRecord {
					Email = _currentEmployee.PrimaryEmail,
					Hash = _currentEmployee.Password.HashValue,
					Time = DateTime.UtcNow
				};
				await _keyValStorage.SetValue (LOGGED_IN_EMPLOYEE_KEY, loginRec);
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Warn);
			}
			return emp;
		}

		public async Task LogOutAsync()
		{
			try{
				if (_currentEmployee != null && _currentRestaurant != null) {
					var ev = _eventFactory.CreateEmployeeLoggedOutOfInventoryAppEvent (_currentEmployee);
					_employeeRepository.ApplyEvent (ev);

					var id = _currentEmployee.Id;
					await _employeeRepository.Commit (id);
				}
			} catch(Exception e){
				_logger.HandleException (e);
			}

			_currentEmployee = null;
			_currentRestaurant = null;
		    await _repositoryLoader.ClearAllRepositories();
		    await _repositoryLoader.LoadRepositories(null);

			//remove our stored employee from our local settings
			await _keyValStorage.DeleteValue(LOGGED_IN_EMPLOYEE_KEY);	
			await _keyValStorage.DeleteValue (LAST_RESTAURANT_ID_KEY);
		}

		public async Task<IEnumerable<IRestaurant>> GetPossibleRestaurantsForLoggedInEmployee ()
		{
			if(_currentEmployee == null){
				throw new InvalidOperationException ("Employee must be logged in to get possible restaurants");
			}

			var results = new List<IRestaurant> ();
			foreach(var restID in _currentEmployee.GetRestaurantIDs ())
			{
				var foundLocal = _restaurantRepository.GetByID (restID);
				if(foundLocal != null){
					results.Add (foundLocal);
				} else{
					await _restaurantRepository.Load (restID);
					foundLocal = _restaurantRepository.GetByID (restID);
					if(foundLocal != null){
						results.Add (foundLocal);
					}
				}
			}

			return results;
		}

		public Task SelectRestaurantForLoggedInEmployee (Guid restaurantID)
		{
			_logger.Debug ("User selected restaurant");
			_currentRestaurant = _restaurantRepository.GetByID (restaurantID);
		    return Task.FromResult(true);
		}

		public async Task LoadSelectedRestaurant ()
		{
			if (_currentRestaurant == null) {
				throw new InvalidOperationException ("No restaurant is selected");
			}

			_eventFactory.SetRestaurant (_currentRestaurant);

            //store the account ID in case we loaded it again.  Sort of a hack, but let's see if this fixes it
            var accountId = _currentRestaurant.AccountID;

			//load the repositories as well to reflect our new restaurant!
			await _repositoryLoader.LoadRepositories(_currentRestaurant.Id);

			_logger.Debug ("Current restaurant is set, creating event");
			var selEv = _eventFactory.CreateUserSelectedRestaurant (_currentEmployee, _currentRestaurant.Id);
			_currentRestaurant = _restaurantRepository.ApplyEvent (selEv);

            if (accountId.HasValue)
            {
                await _accountRepository.LoadAccount(accountId.Value);
            }

            if (accountId.HasValue)
            {
                if ((!_currentRestaurant.AccountID.HasValue) || (_currentRestaurant.AccountID != accountId))
                {
                    var baseRest = _currentRestaurant as Restaurant;
                    if (baseRest != null)
                    {
                        baseRest.AccountID = accountId;
                    }
                }
            }

			await _restaurantRepository.Commit (_currentRestaurant.Id);
			_logger.Debug ("User selected restaurant event committed");

			//store it in the DB
			var restRecord = new RestaurantSelectRecord{RestaurantID = _currentRestaurant.Id};
			await _keyValStorage.SetValue (LAST_RESTAURANT_ID_KEY, restRecord);
		}

		public Task<IEmployee> GetCurrentEmployee ()
		{
			return Task.FromResult (_currentEmployee);
		}

		public Task<IRestaurant> GetCurrentRestaurant ()
		{
			return Task.FromResult (_currentRestaurant);
		}



		public async Task AddNewSectionToRestaurant (string sectionName, bool hasPartialBottles, bool isDefaultInventorySection)
		{
			try{
				if (_currentRestaurant == null) {
					throw new InvalidOperationException ("Do not have a restaurant to add a section to!");
				}

				var existing = _currentRestaurant.GetInventorySections ()
					.Select (s => s.Name)
					.Where(n => string.IsNullOrEmpty (n) == false)
					.Select (n => n.ToUpper ());
				if(existing.Contains (sectionName.ToUpper ())){
					throw new ArgumentException ("Section " + sectionName + " already exists in restaurant");
				}
				var secEvent = 
					_eventFactory.CreateInventorySectionAddedToRestaurantEvent(_currentEmployee, sectionName, 
						isDefaultInventorySection, hasPartialBottles);

				_currentRestaurant = _restaurantRepository.ApplyEvent(secEvent);
				var res = await _restaurantRepository.Commit (_currentRestaurant.Id);
				if(res == CommitResult.Error){
					throw new Exception ("Error committing new section!");
				}
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}


	    public Task InviteEmployeeToUseApp (EmailAddress destEmail)
		{
			try{
			    var res = _currentRestaurant;
				//make the event
				var ev = _eventFactory.CreateEmployeeInvitedToApplicationEvent (_currentEmployee, destEmail, 
					MiseAppTypes.StockboyMobile, res.Name);
				var invite = _inviteRepository.ApplyEvent (ev);

				return _inviteRepository.Commit (invite.Id);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public Task<IEnumerable<IApplicationInvitation>> GetInvitationsForCurrentEmployee ()
		{
			if(_currentEmployee == null){
				throw new InvalidOperationException ();
			}

		    var allInvites = _inviteRepository.GetAll();
			var items = allInvites.Where(i => i.Application == MiseAppTypes.StockboyMobile && i.Status == InvitationStatus.Sent);

		    var empEmails = _currentEmployee.GetEmailAddresses ().ToList();
		    var res = items.Where(item => empEmails.Any(e => e.Equals(item.DestinationEmail))).ToList();

		    //TODO if no items, load from the repos
			return Task.FromResult (res.AsEnumerable());
		}

		public Task<IEnumerable<IApplicationInvitation>> GetPendingInvitationsForRestaurant ()
		{
			var items = _inviteRepository.GetAll ()
				.Where (i => i.Application == MiseAppTypes.StockboyMobile)
				.Where (i => i.RestaurantID == _currentRestaurant.Id);

			return Task.FromResult (items);
		}

		public async Task AcceptInvitation (IApplicationInvitation invite)
		{
			try{
				//make event, pass it to both 
				var acceptEv = _eventFactory.CreateEmployeeAcceptsInvitationEvent (invite, _currentEmployee);

				_inviteRepository.ApplyEvent (acceptEv);
				_currentEmployee = _employeeRepository.ApplyEvent (acceptEv);

				await _inviteRepository.Commit (invite.Id);
				await _employeeRepository.Commit (_currentEmployee.Id);

				await SelectRestaurantForLoggedInEmployee(acceptEv.RestaurantId);
				await LoadSelectedRestaurant();
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public Task RejectInvitation (IApplicationInvitation invite)
		{
			try{
				var ev = _eventFactory.CreateEmployeeRejectsInvitiationEvent (invite, _currentEmployee);

			     _inviteRepository.ApplyEvent (ev);
				return _inviteRepository.Commit (invite.Id);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task<IEmployee> RegisterEmployee (EmailAddress email, Password password, PersonName name)
		{
			try{
				//check if this email is already registered!

				var alreadyRegistered = await _employeeRepository.IsEmailRegistered(email);

				if(alreadyRegistered){
					throw new InvalidOperationException("Email " + email.Value + " is already registered!");
				}

				var ev = _eventFactory.CreateEmployeeCreatedEvent (email, password, name, MiseAppTypes.StockboyMobile);

				_currentEmployee = _employeeRepository.ApplyEvent (ev);
			    var registerEvent = _eventFactory.CreateEmployeeRegisteredForInventoryAppEvent(_currentEmployee);
			    _currentEmployee = _employeeRepository.ApplyEvent(registerEvent);

			    await _employeeRepository.CommitOnlyImmediately(_currentEmployee.Id);


				//reload invitations from the server, we'll need to know this information!
				await _inviteRepository.Load (email);

				return _currentEmployee;	
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task CreatePlaceholderRestaurantForCurrentEmployee ()
		{
			try{
				var ev = _eventFactory.CreatePlaceholderRestaurantCreatedEvent (_currentEmployee);

				_currentRestaurant = _restaurantRepository.ApplyEvent (ev);

				//we do need to commit our repositories!
				if(_restaurantRepository.Dirty){
					await _restaurantRepository.Commit (_currentRestaurant.Id);
				}
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task<IRestaurant> RegisterRestaurant(BusinessName name, StreetAddress address, PhoneNumber phone)
	    {
			try{
		        var ev = _eventFactory.CreateNewRestaurantRegisteredOnAppEvent(_currentEmployee, name, address, phone);
		    
				_currentRestaurant = _restaurantRepository.ApplyEvent (ev);

				//don't commit here - we want to do so only when we register our account, in case there's a problem
				await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.Id);

				//set our system to use this new restaurant!
				_eventFactory.SetRestaurant (_currentRestaurant);

				//associate the restaurant with the employee?
				var empEvent = _eventFactory.CreateEmployeeRegistersRestaurantEvent (_currentEmployee, _currentRestaurant);

				_currentEmployee = _employeeRepository.ApplyEvent (empEvent);

				await _employeeRepository.CommitOnlyImmediately (_currentEmployee.Id);

				return _currentRestaurant;
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task CommitRestaurantRegistrationWithoutAccount ()
		{
			if(_employeeRepository.Dirty){
				await _employeeRepository.CommitOnlyImmediately (_currentEmployee.Id);
			}

			if(_restaurantRepository.Dirty){
				await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.Id);

				await _restaurantRepository.Load(_currentRestaurant.Id);
			}

			await SelectRestaurantForLoggedInEmployee (_currentRestaurant.Id);
		}
			

		private class RegisterAccountInfo{
			public EmailAddress Email;
			public ReferralCode Referral;
			public PersonName AccountName;
			public MiseAppTypes App;
		    public Guid AccountID;
		}

		private RegisterAccountInfo _currentRegistrationInProcess;
		public Task StartRegisterAccount(EmailAddress email, ReferralCode code, PersonName accountName, MiseAppTypes app){
			//just store this information, and return a checksum
			var storedInfo = new RegisterAccountInfo {
				Email = email,
				Referral = code,
				AccountName = accountName,
				App = app,
                AccountID = Guid.NewGuid()
			};

			_currentRegistrationInProcess = storedInfo;
			return Task.FromResult(storedInfo.GetHashCode ());
		}

	    public Task<Guid?> GetRegisteringAccountID()
	    {
	        var accountID = _currentRegistrationInProcess != null ? (Guid?)_currentRegistrationInProcess.AccountID : null;

	        return Task.FromResult(accountID);
	    }

		public async Task<IAccount> RegisterAccount (EmailAddress email, ReferralCode code, PersonName accountName, 
			PhoneNumber phone, MiseAppTypes app, CreditCardNumber cardDetails)
		{
			//auth the credit card.  We'll create the subscription on the server?
			var card = await _ccProcessor.SendCardToProcessorForSubscription(accountName, cardDetails);

			var ev = _eventFactory.CreateAccountRegisteredFromMobileDeviceEvent (_currentEmployee, Guid.NewGuid (), email,
                phone, card, code, app, accountName, MisePaymentPlan.StockboyBasic);

			var acct = _accountRepository.ApplyEvent (ev);
			await _accountRepository.CommitOnlyImmediately (acct.Id);

            if(_employeeRepository.Dirty){
                await _employeeRepository.CommitOnlyImmediately (_currentEmployee.Id);
            }

            //also associate the restaurant with this account
            var restAssignedEvent = _eventFactory.CreateRestaurantAssignedToAccountEvent(_currentEmployee, _currentRestaurant, 
                                        acct);
            _currentRestaurant = _restaurantRepository.ApplyEvent(restAssignedEvent);
                
			await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.Id);

            await SelectRestaurantForLoggedInEmployee(_currentRestaurant.Id);

			return acct;
		}

	    public async Task<IAccount> CompleteRegisterAccount(CreditCard card)
	    {
			if (_currentRegistrationInProcess == null) {
				throw new InvalidOperationException ("No registration currently in process!");
			}

			try{
				//commit account registry
				var ev = _eventFactory.CreateAccountRegisteredFromMobileDeviceEvent (_currentEmployee, _currentRegistrationInProcess.AccountID,
					_currentRegistrationInProcess.Email, _currentRestaurant.PhoneNumber, card, _currentRegistrationInProcess.Referral, 
					_currentRegistrationInProcess.App, _currentRegistrationInProcess.AccountName, MisePaymentPlan.StockboyBasic);

				var acct = _accountRepository.ApplyEvent (ev);
				await _accountRepository.CommitOnlyImmediately (acct.Id);

				if(_employeeRepository.Dirty){
					await _employeeRepository.CommitOnlyImmediately (_currentEmployee.Id);
				}

				if(_restaurantRepository.Dirty){
					await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.Id);

					await _restaurantRepository.Load(_currentRestaurant.Id);
				}

				return acct;
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
	    }

		public async Task ChangePasswordForCurrentEmployee (Password oldPassword, Password newPassword)
		{
			var emp = await GetCurrentEmployee ();

			if (newPassword == null) {
				throw new ArgumentException ("Password is null");
			}

			if (!emp.Password.Equals (oldPassword)) {
				throw new ArgumentException ("Old password is not correct!");
			}

			var ev = _eventFactory.CreateEmployeePasswordChangedEvent (emp, newPassword);

			_currentEmployee = _employeeRepository.ApplyEvent (ev);
			await _employeeRepository.Commit (_currentEmployee.Id);
		}

        private const string eulaKey = "EULA_SHOWN_1.0";
        public bool HasBeenShowEula()
        {
            try{
                var found = _keyValStorage.GetID(eulaKey);
                return found != null;
            } catch(Exception e){
                _logger.HandleException(e);
                return false;
            }
        }


        public Task SetEulaAsShown()
        {
            return _keyValStorage.SetID(eulaKey, Guid.NewGuid());
        }

        private const string InvReminderShownKey = "invShownReminder";
        public bool HasInventoryShownClearReminder(Guid inventoryId)
        {
            try{
                var found = _keyValStorage.GetID(InvReminderShownKey + inventoryId.ToString());
                return found != null;
            } catch(Exception e){
                _logger.HandleException(e);
                return false;
            }
        }

        public Task<EmailAddress> GetCurrentAccountEmail()
        {
            EmailAddress email = null;
            if (_currentRestaurant == null || !_currentRestaurant.AccountID.HasValue)
            {
                return Task.FromResult(email);
            }

            var acct = _accountRepository.GetByID(_currentRestaurant.AccountID.Value);
            if (acct == null)
            {
                return Task.FromResult(email);
            }

            return Task.FromResult(acct.PrimaryEmail);
        }

        public async Task ChangeCurrentRestaurantReportingEmail(EmailAddress email)
        {
            if (_currentRestaurant != null)
            {
                var ev = _eventFactory.CreateRestaurantReportingEmailSetEvent(_currentEmployee, _currentRestaurant, email);
                _currentRestaurant = _restaurantRepository.ApplyEvent(ev);

                await _restaurantRepository.Commit(_currentRestaurant.Id);
            }
        }

        public Task MarkInventoryShownClearReminderAsShown(Guid inventoryId)
        {
            return _keyValStorage.SetID(InvReminderShownKey + inventoryId.ToString(), inventoryId);
        }

        public async Task<IAccount> CancelAccount()
        {
            if (_currentRestaurant == null || !_currentRestaurant.AccountID.HasValue)
            {
                return null;
            }

            var account = _accountRepository.GetByID(_currentRestaurant.AccountID.Value);
            if (account == null)
            {
                return null;
            }

            var ev = _eventFactory.CreateAccountCancelledEvent(_currentEmployee, account, _currentRestaurant);

            _accountRepository.ApplyEvent(ev);
            _restaurantRepository.ApplyEvent(ev);

            await _accountRepository.Commit(account.Id);
            if (_restaurantRepository.Dirty)
            {
                await _restaurantRepository.Commit(_currentRestaurant.Id);
            }

            return account;
        }

        public async Task<bool> DoesCurrentRestaurantHaveValidAccount()
        {
            if (_currentRestaurant == null)
            {
                return false;
            }

            if (!_currentRestaurant.AccountID.HasValue)
            {
                return false;
            }

            var account = _accountRepository.GetByID(_currentRestaurant.AccountID.Value);
            if (account == null)
            {
                return false;
            }

            return true;
        }

        public bool IsCurrentUserAccountOwner
        {
            get
            {
                if (_currentEmployee == null)
                {
                    return false;
                }

                if (_currentRestaurant == null || !_currentRestaurant.AccountID.HasValue)
                {
                    return false;
                }

                var account = _accountRepository.GetByID(_currentRestaurant.AccountID.Value);
                if (account == null || account.PrimaryEmail == null)
                {
                    return false;
                }

                if (_currentEmployee.GetEmailAddresses().Contains(account.PrimaryEmail))
                {
                    return true;
                    //return account.Status != MiseAccountStatus.Cancelled && account.Status != MiseAccountStatus.CancelledFully;
                }

                return false;
            }
        }
	}
}