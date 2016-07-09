using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.People;
using Mise.Core.Services.UtilityServices;
using Xamarin;
using Xamarin.Forms;


using Mise.Inventory.Services;
using Mise.Core.ValueItems;
using Mise.Core.Services;
using Mise.Core.Client.Services;
namespace Mise.Inventory.ViewModels
{
    public class UserRegistrationViewModel : BaseViewModel
    {
        readonly ILoginService _loginService;
        private readonly IInsightsService _insights;
        private readonly ICryptography _crypto;
		public UserRegistrationViewModel(ILoginService login, IAppNavigation navi, ILogger logger, 
			IInsightsService insightsService, ICryptography crypto) : base(navi, logger)
        {
            _loginService = login;
		    _insights = insightsService;
            _crypto = crypto;
			RegisterRestaurant = true;
            SubmitCommand = new Command (Submit);

            PropertyChanged += (sender, e) =>
            {
				try{
	                if (e.PropertyName != "CanSubmit")
	                {
						CanSubmit = EmailAddress.IsValid(Email) && Password.IsValid(PasswordFirst)
	                    && string.IsNullOrEmpty(FirstName) == false
	                    && string.IsNullOrEmpty(LastName) == false
	                    && PasswordFirst == PasswordRepeat
						&& NotProcessing;
	                }
				} catch(Exception ex){
					HandleException (ex);
				}
            };
        }

        public Action LoadDataOnForm { get; set; }

        public override Task OnAppearing()
        {
            if (LoadDataOnForm != null)
            {
                LoadDataOnForm();
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Email
        {
            get { return GetValue<string>(); }
            set { SetValue<string>(value); }
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string PasswordFirst
        {
            get { return GetValue<string>(); }
            set { SetValue<string>(value); }
        }

        /// <summary>
        /// Gets or sets the password check field
        /// </summary>
        public string PasswordRepeat
        {
            get { return GetValue<string>(); }
            set { SetValue<string>(value); }
        }

        public string FirstName
        {
            get { return GetValue<string>(); }
            set { SetValue<string>(value); }
        }

        public string LastName
        {
            get { return GetValue<string>(); }
            set { SetValue<string>(value); }
        }

		public bool RegisterRestaurant{get{return GetValue<bool> ();}set{ SetValue (value); }}

        public bool CanSubmit { get { return GetValue<bool>(); } set { SetValue(value); } }

        public ICommand SubmitCommand { get; set; }

        public async void EmailAlreadyTaken(SendEventsException e)
        {
			await DisplayMessageModal ("Already Registered", "Email is already registered");
            Email = string.Empty;
        }

        public async void Submit()
        {
            var email = new EmailAddress { Value = Email };
            var password = new Password(PasswordFirst, _crypto);

            try
            {
                var name = new PersonName(FirstName, LastName);
				Processing = true;
                try
                {
                    var emp = await _loginService.RegisterEmployee(email, password, name);
                    if (emp != null)
                    {
                        _insights.Track("Registered User", "User Email", email.Value);
                        _insights.Identify(emp, "Stockboy Mobile");
                    }
                    else
                    {
                        throw new Exception("Failed to register employee!");
                    }
                }
                catch (SendEventsException se)
                {
					if(se.Error == SendErrors.EmailAlreadyInUse){
	                   EmailAlreadyTaken(se);
					} else {
						HandleException (se);
					}
					return;
                }
					
                //see if we need to display invitations or not
                var invites = await _loginService.GetInvitationsForCurrentEmployee();
				Processing = false;
                if (invites.Any())
                {
                    await Navigation.ShowInvitations();
                }
                else
                {
                    //create a placeholder restaurant?
					if(RegisterRestaurant){
						await Navigation.ShowRestaurantRegistration ();
					} else{
                    	await _loginService.CreatePlaceholderRestaurantForCurrentEmployee();
                    	await Navigation.ShowMainMenu();
					}
                }
            }
            catch (Exception e)
            {
				HandleException (e);
            }
        }
    }
}

