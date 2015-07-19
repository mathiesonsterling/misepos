using System;
using System.Threading.Tasks;
using Mise.Inventory.ViewModels;
using Mise.Core.Services;
using Mise.Inventory.Services;
using System.Windows.Input;
using Mise.Inventory.MVVM;
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
		readonly ICreditCardProcessorService _ccProcessor;
		public AccountRegistrationViewModel(ILogger logger, ILoginService loginService, IAppNavigation appNav, 
			IInsightsService insights, ICreditCardProcessorService ccProcessor)
			:base(appNav, logger)
		{
			_loginService = loginService;
			_insights = insights;
			_ccProcessor = ccProcessor;
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

		bool IsFormValid(){
			var res = CreditCard.CardNumberIsValid (CardNumber);

			int month;
			int year;
			if(int.TryParse (BillingMonth, out month))
			{
				if(int.TryParse (BillingYear, out year)){
					res = res && month > 0 && month < 13 && year > 0;
				} else {
					return false;
				}
			} else {
				return false;
			}
			res = res && ZipCode.IsValid (BillingZip);
			int csv;
			if (int.TryParse (CSV, out csv)) {
				res = res && csv > 100 && csv < 1000;
			} else {
				return false;
			}
			res = res && string.IsNullOrEmpty (FirstName) == false;
			res = res && string.IsNullOrEmpty (LastName) == false;

			return res;
		}

		public string CardNumber{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string BillingMonth{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string BillingYear{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string CSV{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string BillingZip{get{return GetValue<string> ();}set{ SetValue (value); }}

		public string FirstName{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string LastName{get{return GetValue<string>();}set{ SetValue (value); }}

		public string ReferralCode{get{return GetValue<string> ();}set{SetValue(value);}}
		public bool CanRegister{get{return GetValue<bool> ();}set{ SetValue (value); }}

		public ICommand RegisterAccountCommand{get{return new SimpleCommand (RegisterAccount);}}
		public ICommand DelayRegistrationCommand{get{return new SimpleCommand (DelayRegistration);}}

		private async void RegisterAccount(){
			try{
				var name = new PersonName (FirstName, LastName);
				var zip = new ZipCode { Value = BillingZip };
				ReferralCode refCode = null;
				if(string.IsNullOrEmpty (ReferralCode) == false){
					refCode = new Mise.Core.ValueItems.ReferralCode (ReferralCode);
				}

				Processing = true;
				//get the token from billing service
				var ccNumber = new CreditCardNumber(CardNumber, int.Parse(CSV), int.Parse(BillingZip));
				var processedCard = await _ccProcessor.AuthorizeCard (ccNumber);
				if(processedCard == null){
					Processing = false;
					await Navigation.DisplayAlert ("Error","Credit card could not be authorized");
					this.CardNumber= string.Empty;
					CSV = string.Empty;
					return;
				}
				var acct = await _loginService.RegisterAccount (processedCard, refCode, name, MiseAppTypes.StockboyMobile);
				if(acct != null){
					_insights.Track ("User Registered", "User ID", acct.ID.ToString ());
				}
				Processing = false;
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

	    public override Task OnAppearing()
	    {
	        return Task.FromResult(false);
	    }
	}
}

