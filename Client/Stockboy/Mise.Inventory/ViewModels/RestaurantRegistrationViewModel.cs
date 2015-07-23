using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using System.Windows.Input;
using Mise.Inventory.MVVM;
using Mise.Core.Services;
using Mise.Inventory.Services;


namespace Mise.Inventory.ViewModels
{
	public class RestaurantRegistrationViewModel : BaseViewModel
	{
		readonly ILoginService _login;
		public RestaurantRegistrationViewModel(ILogger logger, ILoginService login, IAppNavigation nav) : base(nav, logger)
		{
			_login = login;

			PropertyChanged += (sender, e) => {
				if(e.PropertyName !=  "CanAdd"){
					//don't need street direction
					CanAdd = 
						string.IsNullOrEmpty (Name) == false
						&& string.IsNullOrEmpty (StreetAddressNumber) == false
						&& string.IsNullOrEmpty (StreetName) == false
						&& string.IsNullOrEmpty (City) == false
						&& string.IsNullOrEmpty (State) == false
						&& string.IsNullOrEmpty (Zip) == false
						&& string.IsNullOrEmpty (PhoneAreaCode) == false
						&& string.IsNullOrEmpty (PhoneNumberVal) == false;
				}
			};
		}

        public override async Task OnAppearing()
        {
			Processing = true;
			var emp = await _login.GetCurrentEmployee ();
			if (emp != null && emp.PrimaryEmail != null) {
				EmailToGetReportsAt = emp.PrimaryEmail.Value;
			}
			Processing = false;
        }

		#region Fields
		public bool CanAdd{get{return GetValue<bool> ();}set{ SetValue (value); }}
		public string Name{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string StreetAddressNumber{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string StreetDirection{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string StreetName{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string City{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string State{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string Zip{get{return GetValue<string> ();}set{ SetValue (value); }}

		public string PhoneAreaCode{get{return GetValue<string> ();}set{ SetValue (value); }}
		public string PhoneNumberVal{get{return GetValue<string> ();}set{ SetValue (value); }}

		public string EmailToGetReportsAt{get{ return GetValue<string> (); }set{SetValue(value);}}

		public IEnumerable<State> States{get{return Mise.Core.ValueItems.State.GetUSStates ();}}
		#endregion

		public ICommand RegisterRestaurantCommand{get{return new SimpleCommand (Register);}}

		public async void Register(){
			try{
				var phone = new PhoneNumber (PhoneAreaCode, PhoneNumberVal);
				var state = States.FirstOrDefault(s => s.Abbreviation == State);
				var address = new StreetAddress {
					StreetAddressNumber = new Mise.Core.ValueItems.StreetAddressNumber{
						Number = StreetAddressNumber, 
						Direction = StreetDirection
					},
					Street = new Street{Name = StreetName},
					City = new City{Name = City},
					State = state,
					Zip = new ZipCode{Value = Zip},
					Country = Country.UnitedStates
				};

				var name = new RestaurantName (Name);
				Processing = true;
				await _login.RegisterRestaurant (name, address, phone);
				Processing = false;
			} catch(Exception e){
				HandleException (e);
			}

			await Navigation.ShowAccountRegistration ();
		}
	}
}

