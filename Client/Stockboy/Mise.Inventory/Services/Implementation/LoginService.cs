using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

using Mise.Core.Common.Events;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Client.Services;

namespace Mise.Inventory.Services.Implementation
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

		IEmployee _currentEmployee;
		IRestaurant _currentRestaurant;
		IRestaurantInventorySection _currentSection;

		public class MiseLoginRecord{
			public Guid EmployeeID{get;set;}
			public DateTime Time{get;set;}
		}

		public LoginService(IEmployeeRepository employeeRepository,
							IRestaurantRepository restaurantRepository,
							IApplicationInvitationRepository inviteRepository,
							IAccountRepository accountRepository,
		                    IInventoryAppEventFactory eventFactory,
							IClientKeyValueStorage keyValStorage,
		                    ILogger logger,
            IRepositoryLoader repositoryLoader
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
		}

		const string LOGGED_IN_EMPLOYEE_ID_KEY = "LoggedInEmployee";
		public void OnAppStarting(){
			try{
				//if we have an employee that logged in, less than 7 days ago, then mark it
				var login = _keyValStorage.GetValue<MiseLoginRecord> (LOGGED_IN_EMPLOYEE_ID_KEY);
				if(login != null){
					if (login.Time > DateTime.UtcNow.AddDays (-7)) {
						_currentEmployee = _employeeRepository.GetByID (login.EmployeeID);
					}
				}
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Warn);
			}
		}

		/// <summary>
		/// Sets the current employee.  Used only for testing!
		/// </summary>
		/// <param name="emp">Emp.</param>
		public void SetCurrentEmployee(IEmployee emp){
			_currentEmployee = emp;
		}

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
			await _employeeRepository.Commit(emp.ID);

			_currentEmployee = emp;

            //update the application invitation repository with this email!
		    await _inviteRepository.Load(email);

			try{
				var loginRec = new MiseLoginRecord {
					EmployeeID = _currentEmployee.ID,
					Time = DateTime.UtcNow
				};
				_keyValStorage.SetValue (LOGGED_IN_EMPLOYEE_ID_KEY, loginRec);
			} catch(Exception e){
				_logger.HandleException (e, LogLevel.Warn);
			}
			return emp;
		}

		public async Task LogOutAsync()
		{
			if (_currentEmployee != null) {
				var ev = _eventFactory.CreateEmployeeLoggedOutOfInventoryAppEvent (_currentEmployee);
				_employeeRepository.ApplyEvent (ev);

				var id = _currentEmployee.ID;
				await _employeeRepository.Commit (id);
			}

			_currentEmployee = null;

		    await _repositoryLoader.ClearAllRepositories();
		    await _repositoryLoader.LoadRepositories(null);

			//remove our stored employee from our local settings
			_keyValStorage.DeleteValue(LOGGED_IN_EMPLOYEE_ID_KEY);				
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

		public async Task SelectRestaurantForLoggedInEmployee (Guid restaurantID)
		{
			_logger.Debug ("User selected restaurant");
			_currentRestaurant = _restaurantRepository.GetByID (restaurantID);

			if (_currentRestaurant != null) {
				
				_eventFactory.SetRestaurant (_currentRestaurant);

				//load the repositories as well to reflect our new restaurant!
			    await _repositoryLoader.LoadRepositories(_currentRestaurant.ID);

				_logger.Debug ("Current restaurant is set, creating event");
				var selEv = _eventFactory.CreateUserSelectedRestaurant (_currentEmployee, restaurantID);
				_currentRestaurant = _restaurantRepository.ApplyEvent (selEv);
				await _restaurantRepository.Commit (_currentRestaurant.ID);
				_logger.Debug ("User selected restaurant event committed");
			} else{
				_logger.Error("Current restaurant is null!");
			}
		}

		public Task<IEmployee> GetCurrentEmployee ()
		{
			return Task.FromResult (_currentEmployee);
		}

		public Task<IRestaurant> GetCurrentRestaurant ()
		{
			return Task.FromResult (_currentRestaurant);
		}

		public Task<IRestaurantInventorySection> GetCurrentSection ()
		{
			return Task.FromResult (_currentSection);
		}

		public Task SelectSection (IRestaurantInventorySection section)
		{
			_currentSection = section;
			return Task.FromResult (false);
			//ensure section and restaurant match!
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
				var res = await _restaurantRepository.Commit (_currentRestaurant.ID);
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

				return _inviteRepository.Commit (invite.ID);
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
		    var items = allInvites.Where(i => i.Application == MiseAppTypes.StockboyMobile);

		    var empEmails = _currentEmployee.GetEmailAddresses ().ToList();
		    var res = items.Where(item => empEmails.Any(e => e.Equals(item.DestinationEmail))).ToList();

		    //TODO if no items, load from the repos
			return Task.FromResult (res.AsEnumerable());
		}

		public Task<IEnumerable<IApplicationInvitation>> GetPendingInvitationsForRestaurant (Guid restaurantID)
		{
			var items = _inviteRepository.GetAll ()
				.Where (i => i.Application == MiseAppTypes.StockboyMobile)
				.Where (i => i.RestaurantID == restaurantID);

			return Task.FromResult (items);
		}

		public async Task AcceptInvitation (IApplicationInvitation invite)
		{
			try{
				//make event, pass it to both 
				var acceptEv = _eventFactory.CreateEmployeeAcceptsInvitationEvent (invite, _currentEmployee);

				_inviteRepository.ApplyEvent (acceptEv);
				_currentEmployee = _employeeRepository.ApplyEvent (acceptEv);

				await _inviteRepository.Commit (invite.ID);
				await _employeeRepository.Commit (_currentEmployee.ID);
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
				return _inviteRepository.Commit (invite.ID);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task<IEmployee> RegisterEmployee (EmailAddress email, Password password, PersonName name)
		{
			try{
				var ev = _eventFactory.CreateEmployeeCreatedEvent (email, password, name);

				_currentEmployee = _employeeRepository.ApplyEvent (ev);


			    await _employeeRepository.CommitOnlyImmediately(_currentEmployee.ID);


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
					await _restaurantRepository.Commit (_currentRestaurant.ID);
				}
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public Task<IRestaurant> RegisterRestaurant(RestaurantName name, StreetAddress address, PhoneNumber phone)
	    {
			try{
		        var ev = _eventFactory.CreateNewRestaurantRegisteredOnAppEvent(_currentEmployee, name, address, phone);
		    
				_currentRestaurant = _restaurantRepository.ApplyEvent (ev);

				//don't commit here - we want to do so only when we register our account, in case there's a problem
				//await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.ID);

				//set our system to use this new restaurant!
				_eventFactory.SetRestaurant (_currentRestaurant);

				//associate the restaurant with the employee?
				_currentEmployee = _employeeRepository.ApplyEvent (ev);

				return Task.FromResult(_currentRestaurant);
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
		}

		public async Task CommitRestaurantRegistrationWithoutAccount ()
		{
			if(_employeeRepository.Dirty){
				await _employeeRepository.CommitOnlyImmediately (_currentEmployee.ID);
			}

			if(_restaurantRepository.Dirty){
				await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.ID);

				await _restaurantRepository.Load(_currentRestaurant.ID);
			}
		}
			
	    public async Task<IAccount> RegisterAccount(CreditCard card, ReferralCode code, PersonName name, MiseAppTypes app)
	    {
			try{
				//send our credit card, to get the token


				//commit account registry
				var ev = _eventFactory.CreateAccountRegisteredFromMobileDeviceEvent (_currentEmployee, 
					_currentEmployee.PrimaryEmail, _currentRestaurant.PhoneNumber, card, code, app, name);

				var acct = _accountRepository.ApplyEvent (ev);
				await _accountRepository.CommitOnlyImmediately (acct.ID);

				if(_employeeRepository.Dirty){
					await _employeeRepository.CommitOnlyImmediately (_currentEmployee.ID);
				}

				if(_restaurantRepository.Dirty){
					await _restaurantRepository.CommitOnlyImmediately (_currentRestaurant.ID);

					await _restaurantRepository.Load(_currentRestaurant.ID);
				}

				return acct;
			} catch(Exception e){
				_logger.HandleException (e);
				throw;
			}
	    }
	}
}