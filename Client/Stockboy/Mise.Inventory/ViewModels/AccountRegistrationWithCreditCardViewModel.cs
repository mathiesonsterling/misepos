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

			CanRegister = false;
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
			Number = string.Empty;
            CSV = string.Empty;
            ExpMonth = DateTime.Now.Month;
            ExpYear = DateTime.Now.Year;

			Processing = false;
		}

		#endregion
		public string FirstName{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string LastName{get{return GetValue<string>();}set{ SetValue (value); }}
		public string Email{get{ return GetValue<string> (); }set{SetValue(value);}}
		public string ReferralCode{get{return GetValue<string> ();}set{SetValue(value);}}

		public string Number{get{ return GetValue<string>(); }set{ SetValue(value); }}
		public string CSV{get{ return GetValue<string>(); }set{ SetValue(value); }}
		public string ZipCode{get{ return GetValue<string>(); }set{ SetValue(value); }}

        public int ExpMonth{get{ return GetValue<int>(); }set{ SetValue(value); }}
        public int ExpYear{get{return GetValue<int>();}set{SetValue(value);}}

		public bool CanRegister{get{return GetValue<bool> ();}set{ SetValue (value); }}

		public ICommand RegisterAccountCommand{
			get{return new Command (RegisterAccount, () => CanRegister);}
		}
		public ICommand DelayRegistrationCommand{
			get{return new Command (DelayRegistration, () => NotProcessing);}
		}

		bool IsFormValid(){
			var res = EmailAddress.IsValid (Email);
			res = res && !string.IsNullOrWhiteSpace (FirstName);
			res = res && !string.IsNullOrWhiteSpace(LastName);
			res = res && !string.IsNullOrWhiteSpace (Number);
            res = res && CreditCard.CardNumberIsValid(Number);
			res = res && !string.IsNullOrWhiteSpace(CSV) && !string.IsNullOrWhiteSpace(ZipCode);
            res = res && (ExpYear >= DateTime.Now.Year || (ExpYear < 2000 && ExpYear >= DateTime.Now.Year - 2000));
			return res;
		}

		private async void RegisterAccount(){
			try{
				var name = new PersonName (FirstName, LastName);
				var email = new EmailAddress(Email);
				ReferralCode refCode = null;
				if(string.IsNullOrEmpty (ReferralCode) == false){
					refCode = new ReferralCode (ReferralCode);
				}

				int csvN;
				int zip;
				if(!int.TryParse(CSV, out csvN))
				{
					CSV = string.Empty;
					throw new ArgumentException("Invalid CSV code");
				}
				if(!int.TryParse(ZipCode, out zip)){
					ZipCode = string.Empty;
					throw new ArgumentException("Invalid ZipCode");
				}

                var zipE = new ZipCode{Value = zip.ToString()};
				var cardNumber = new CreditCardNumber(Number, csvN, zipE, ExpMonth, ExpYear);

				Processing = true;
                CanRegister = false;
				//store our info here, we'll complete it once the webpage returns
				_insights.Track ("User Began Registration", "Email", email.Value);
				await _loginService.RegisterAccount(email, refCode, name, null, MiseAppTypes.StockboyMobile, cardNumber); 
				Processing = false;
                CanRegister = true;

				//go to the webpage
                await base.DisplayMessageModal("Account Created", "Your account has been created!");

                await Navigation.ShowRestaurantLoading ();

			} catch(Exception e){
				HandleException (e);
                CanRegister = true;
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

