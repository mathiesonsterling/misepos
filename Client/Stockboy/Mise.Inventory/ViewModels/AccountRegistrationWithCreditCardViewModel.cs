using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
namespace Mise.Inventory.ViewModels
{
	public class AccountRegistrationWithCreditCardViewModel : BaseViewModel
	{
		private readonly ILoginService _loginService;
		private readonly IInsightsService _insights;
		public AccountRegistrationWithCreditCardViewModel (ILoginService loginService, IInsightsService insights,
			IAppNavigation navi, ILogger logger) : base(navi, logger)
		{
			_loginService = loginService;
			_insights = insights;
		}

		#region implemented abstract members of BaseViewModel

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

		#endregion
		public string FirstName{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string LastName{get{return GetValue<string>();}set{ SetValue (value); }}
		public string Email{get{ return GetValue<string> (); }set{SetValue(value);}}
		public string ReferralCode{get{return GetValue<string> ();}set{SetValue(value);}}
		public bool CanRegister{get{return GetValue<bool> ();}set{ SetValue (value); }}

		public string Number{get{ return GetValue<string>(); }set{ SetValue(value); }}
		public int CSV{get{ return GetValue<int>(); }set{ SetValue(value); }}
		public int ZipCode{get{ return GetValue<int>(); }set{ SetValue(value); }}

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

				var cardNumber = new CreditCardNumber(Number, CSV, ZipCode);

				Processing = true;
				//store our info here, we'll complete it once the webpage returns
				_insights.Track ("User Began Registration", "Email", email.Value);
				await _loginService.RegisterAccount(email, refCode, name, null, MiseAppTypes.StockboyMobile, cardNumber); 
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
					_insights.Track("Delayed Registration", "EmpID", emp.Id.ToString ());
				}
				await Navigation.ShowRestaurantLoading();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

