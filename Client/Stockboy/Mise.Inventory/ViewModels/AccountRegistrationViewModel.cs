using System;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.ViewModels;
using Mise.Core.Services;
using Mise.Inventory.Services;
using System.Windows.Input;

using Mise.Core.ValueItems;
using Xamarin.Forms;
using Mise.Core.Entities;
using System.Threading;
using Mise.Core;

namespace Mise.Inventory.ViewModels
{
	public class AccountRegistrationViewModel : BaseViewModel
	{
		readonly ILoginService _loginService;
		readonly IInsightsService _insights;
		public AccountRegistrationViewModel(ILogger logger, ILoginService loginService, IAppNavigation appNav, 
			IInsightsService insights)
			:base(appNav, logger)
		{
			_loginService = loginService;
			_insights = insights;

			PropertyChanged += (sender, e) => {
				try{
					if(e.PropertyName != "CanRegister"){
						//check credit card, etc are valid
						CanRegister = IsFormValid ();
					}
				} catch(Exception ex){
					HandleException (ex);
				}
			};
		}

		public override async Task OnAppearing()
		{
			Processing = true;
			var emp = await _loginService.GetCurrentEmployee ();
			if (emp != null ) {
				if (emp.PrimaryEmail != null) {
					Email = emp.PrimaryEmail.Value;
				}
				if (emp.Name != null) {
					FirstName = emp.Name.FirstName;
					LastName = emp.Name.LastName;
				}
			}
			Processing = false;
		}

		bool IsFormValid(){
			var res = EmailAddress.IsValid (Email);
			res = res && string.IsNullOrEmpty (FirstName) == false;
			res = res && string.IsNullOrEmpty (LastName) == false;

			return res;
		}

		public string FirstName{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string LastName{get{return GetValue<string>();}set{ SetValue (value); }}
		public string Email{get{ return GetValue<string> (); }set{SetValue(value);}}
		public string ReferralCode{get{return GetValue<string> ();}set{SetValue(value);}}
		public bool CanRegister{get{return GetValue<bool> ();}set{ SetValue (value); }}

		public ICommand RegisterAccountCommand{
			get{return new Command (RegisterAccount, () => CanRegister);}
		}
		public ICommand DelayRegistrationCommand{
			get{return new Command (DelayRegistration, () => NotProcessing);}
		}

		private async void RegisterAccount(){
			try{
				var name = new PersonName (FirstName, LastName);
				var email = new EmailAddress(Email);
				ReferralCode refCode = null;
				if(string.IsNullOrEmpty (ReferralCode) == false){
					refCode = new ReferralCode (ReferralCode);
				}

				Processing = true;
				//store our info here, we'll complete it once the webpage returns
				await _loginService.StartRegisterAccount(email, refCode, name, MiseAppTypes.StockboyMobile);
				_insights.Track ("User Began Registration", "Email", email.Value);
				Processing = false;

				//go to the webpage
				await Navigation.ShowAuthorizeCreditCard();

			} catch(Exception e){
				HandleException (e);
			}
		}

		async void DelayRegistration(){
			try{
				await _loginService.CommitRestaurantRegistrationWithoutAccount ();
				var emp = await _loginService.GetCurrentEmployee ();
				if(emp != null){
					_insights.Track("Delayed Registration", "EmpID", emp.ID.ToString ());
				}
				await Navigation.ShowMainMenu ();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

