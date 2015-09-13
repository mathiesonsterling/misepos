using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.ValueItems;
using Mise.Core.Entities.People;
using System.Net;
using Xamarin.Forms;
using Mise.Inventory.Services.Implementation.WebServiceClients.Exceptions;
using Mise.Core.Common.Services.WebServices.Exceptions;


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
	                CanLogin = NotProcessing && EmailAddress.IsValid(Username) 
						&& Core.ValueItems.Password.IsValid(Password);
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
			get { return new Command(LoginWrapper, () => CanLogin); }
		}

		public ICommand RegisterCommand{
			get{return new Command (Register, () => NotProcessing);}
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

		private enum LoginResult{
			ServerError,

			LoggedIn,
			BadPassword,
			BadEmail,
			ServerOffline,
			Other
		}

		/// <summary>
		/// Login this instance.
		/// </summary>
		public async Task Login()
		{
			var result = LoginResult.Other;
			try{
				Processing = true;
				var email = new EmailAddress{ Value = Username.Trim() };
				var password = new Password();
				password.SetValue(Password);
				IEmployee employee = null;

				employee = await _loginService.LoginAsync(email, password);
				Processing = false;

			    if (employee == null)
			    {
			        Password = string.Empty;
			        Username = string.Empty;
			        try
			        {
						await DisplayMessageModal ("Login Error", "Incorrect login information");
			        }
			        catch (Exception e)
			        {
						Logger.HandleException (e);
			        }
			    }
			    else
			    {
					result = LoginResult.LoggedIn;

					_insightsService.Identify(employee, App.DeviceID);

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
						await Navigation.ShowRestaurantLoading ();
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
						Processing = true;
			            var invites = await _loginService.GetInvitationsForCurrentEmployee();
						Processing = false;
			            if (invites.Any())
			            {
			                await Navigation.ShowInvitations();
			            }
			            else
			            {
							//double check this
							var userConf = await AskUserQuestionModal ("No Restaurant Found", 
								"We don't show any restaurants assigned to this user.  This could be a server error.  Do you want to register a new restaurant?");
							if(userConf){
								await Navigation.ShowRestaurantRegistration();
							} 
			            }
			        }
			    }
			} 
			catch(UserNotFoundException unf){
				_insightsService.ReportException (unf, LogLevel.Info);
				if(unf.IncorrectPassword && (unf.NoEmailFound == false)){
					result = LoginResult.BadPassword;
				} else {
					result = LoginResult.BadEmail;
				}
			}
			catch(WebException we){
				_insightsService.ReportException (we, LogLevel.Error);
				Logger.HandleException (we);
				if(we.Message.Contains ("NameResolutionFailure"))
				{
					result = LoginResult.ServerOffline;
				} 
			}
			catch(DataNotSavedOnServerException dse){
				_insightsService.ReportException (dse, LogLevel.Error);
				Logger.HandleException (dse);
				result = LoginResult.ServerError;
			}

			catch(Exception e){
				_insightsService.ReportException (e, LogLevel.Error);
				Logger.HandleException (e);
			}

			if(result != LoginResult.LoggedIn){
				Processing = false;
				switch(result){
				case LoginResult.BadEmail:
					await DisplayMessageModal("User not found", "No user found for " + Username);
					Username = string.Empty;
					Password = string.Empty;
					break;
				case LoginResult.BadPassword:
					await DisplayMessageModal("Incorrect password", "Password is not correct");
					Password = string.Empty;
					break;
				case LoginResult.ServerOffline:
					await DisplayMessageModal("Server offline", "Cannot connect to the server, are you online?");
					break;
				case LoginResult.ServerError:
					await DisplayMessageModal("Server error", "Error while updating server.  We'll be working on getting you back online as quick as possible!");
					break;
				case LoginResult.Other:
					await DisplayMessageModal("Error", "Error logging in.  If this continues try uninstalling and reinstalling Stockboy");
					break;
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
