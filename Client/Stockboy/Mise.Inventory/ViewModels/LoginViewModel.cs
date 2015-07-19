using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;

using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services;
using Mise.Core.Client.Services;
using Mise.Core.Entities.People;
using System.Net.Http;
using Xamarin;


namespace Mise.Inventory.ViewModels
{
	public class LoginViewModel : BaseViewModel
	{
		readonly ILoginService _loginService;
	    private readonly IInsightsService _insightsService;

	    /// <summary>
	    /// Initializes a new instance of the <see cref="Mise.Inventory.ViewModels.LoginViewModel"/> class.
	    /// </summary>
	    /// <param name = "logger"></param>
	    /// <param name="loginService">Login service.</param>
	    /// <param name="navigationService">Navigation service.</param>
	    /// <param name="insightsService"></param>
	    public LoginViewModel(ILogger logger, ILoginService loginService, IAppNavigation navigationService, IInsightsService insightsService)
			:base(navigationService, logger)
		{
			_loginService = loginService;
		    _insightsService = insightsService;

	        PropertyChanged += (sender, args) =>
	        {
	            if (args.PropertyName != "CanLogin")
	            {
	                CanLogin = EmailAddress.IsValid(Username) && Core.ValueItems.Password.IsValid(Password);
	            }
	        };
		}

		public override async Task OnAppearing(){
			var emp = await _loginService.GetCurrentEmployee ();
			if (emp != null) {
				await Navigation.ShowMainMenu ();
			}
		}

        public bool CanLogin { get { return GetValue<bool>(); } private set { SetValue(value);} }

		/// <summary>
		/// Gets the login command.
		/// </summary>
		/// <value>The login command.</value>
		public ICommand LoginCommand {
			get { return new SimpleCommand(LoginWrapper); }
		}

		public ICommand RegisterCommand{
			get{return new SimpleCommand (Register);}
		}

	    public bool NotProduction
	    {
	        get { return DependencySetup.GetBuildLevel() != Mise.Inventory.BuildLevel.Production; }
	    }

        public string BuildLevel { get { return DependencySetup.GetBuildLevel().ToString(); } }

	    /// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
		public string Password {
			get { return GetValue<string>(); }
			set { SetValue<string>(value); }
		}

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
		public string Username {
			get { return GetValue<string>(); }
			set { SetValue<string>(value); }
		}

	    private async void LoginWrapper()
	    {
	        await Login();
	    }

		/// <summary>
		/// Login this instance.
		/// </summary>
		public async Task Login()
		{
		    bool succeeded = false;
		    bool shownErrorMessage = false;
			try{
				Processing = true;
				var email = new EmailAddress{ Value = Username.Trim() };
				var password = new Password();
				password.SetValue(Password);
				IEmployee employee = null;
				try{
					employee = await _loginService.LoginAsync(email, password);
				} catch(HttpRequestException he){
					//is this a 404?
					if(he.Message.Contains("No employee found for")){
						_insightsService.Track("No employee found", new Dictionary<string, string>{{"email", email.Value}});
					} else{
						throw;
					}
				}
				Processing = false;

			    if (employee == null)
			    {
			        Password = string.Empty;
			        Username = string.Empty;
			        try
			        {
			            await Navigation.DisplayAlert("Login Error", "Incorrect login information");
			            shownErrorMessage = true;
			        }
			        catch (Exception e)
			        {
						Logger.HandleException (e);
			        }
			    }
			    else
			    {
			        succeeded = true;

			        _insightsService.Identify("Unique user ID", "MiseID", employee.ID.ToString());
			        _insightsService.Identify("Unique user ID", Insights.Traits.Email, email.Value);

					if(employee.Name != null){
						_insightsService.Identify ("First Name", Insights.Traits.FirstName, employee.Name.FirstName);
						_insightsService.Identify ("Last Name", Insights.Traits.LastName, employee.Name.LastName);
					}

			        //do we have more than one restaurant?
			        Processing = true;
			        var restsE = await _loginService.GetPossibleRestaurantsForLoggedInEmployee();

			        var rests = restsE.ToList();
			        var numRests = rests.Count();

			        _insightsService.Track("User Logged In", new Dictionary<string, string> {{"Email", email.Value}});
			        Processing = false;
			        if (numRests == 1)
			        {
			            //set our restaurant
						Processing = true;
			            await _loginService.SelectRestaurantForLoggedInEmployee(rests.First().ID);
						Processing = false;
			            await Navigation.ShowMainMenu();
			            return;
			        }
			        if (numRests > 1)
			        {
			            await Navigation.ShowSelectRestaurant();
			            return;
			        }

			        //if less than one, we need to register with one
			        if (numRests == 0)
			        {
			            //do we have invites?
			            var invites = await _loginService.GetInvitationsForCurrentEmployee();
			            if (invites.Any())
			            {
			                await Navigation.ShowInvitations();
			            }
			            else
			            {
							//double check this
							var userConf = await Navigation.AskUser ("No Restaurant Found", 
								"We don't show any restaurants assigned to this user.  This could be a server error.  Do you want to register a new restaurant?");
							if(userConf){
								await Navigation.ShowRestaurantRegistration();
							} 
			            }
			        }
			    }
			} catch(Exception e){
				_insightsService.ReportException (e, LogLevel.Warn);
				Logger.HandleException (e);
			}

		    if (succeeded == false && shownErrorMessage == false)
		    {
		        try
		        {
					_insightsService.Track("Login error", new Dictionary<string, string>());
		            await Navigation.DisplayAlert("Error", "Error logging in");
		            shownErrorMessage = true;
					Processing = false;
		        }
		        catch (Exception e)
		        {
					HandleException (e);
		        }
		    }
		}

		public async void Register(){
			try{
				var destVM = App.UserRegistrationViewModel;
				if(destVM != null){
					destVM.Email = Username;
					destVM.PasswordFirst = Password;
				}
				await Navigation.ShowUserRegistration ();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}
