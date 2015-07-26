using System.Windows.Input;
using System.Collections.Generic;

using Mise.Core.Repositories;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using Xamarin.Forms;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
    public class VendorAddViewModel : BaseViewModel
    {
        readonly IVendorService _vendorService;
        readonly ILoginService _loginService;
        public VendorAddViewModel(IAppNavigation appNavigation, IVendorService vendorService, ILoginService loginService,
            ILogger logger)
            : base(appNavigation, logger)
        {
            _vendorService = vendorService;
            _loginService = loginService;

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "CanAdd")
                {
                    //do we have a valid address?
                    var add = MakeAddress();
                    var phone = MakePhoneNumber();
                    var email = MakeEmail();

                    //we need EITHER a street address or an email
                    CanAdd = NotProcessing && phone != null && (string.IsNullOrEmpty(Name) == false)
                             && (add != null || email != null);
                }
            };
        }

        public override async Task OnAppearing()
        {
            Email = string.Empty;
            Name = string.Empty;
            //get our restaurant to populate some of the address
            var rest = await _loginService.GetCurrentRestaurant();
            if (rest != null && rest.StreetAddress != null)
            {
                City = rest.StreetAddress.City.Name;
                State = rest.StreetAddress.State.Name;
            }
        }

        #region Fields
        public bool CanAdd { get { return GetValue<bool>(); } set { SetValue(value); } }
        public string Name { get { return GetValue<string>(); } set { SetValue(value); } }

        public string Email { get { return GetValue<string>(); } set { SetValue(value); } }
        public string StreetAddressNumber { get { return GetValue<string>(); } set { SetValue(value); } }
        public string StreetName { get { return GetValue<string>(); } set { SetValue(value); } }
        public string City { get { return GetValue<string>(); } set { SetValue(value); } }
        public string State { get { return GetValue<string>(); } set { SetValue(value); } }
        public string Zip { get { return GetValue<string>(); } set { SetValue(value); } }

        public string PhoneAreaCode { get { return GetValue<string>(); } set { SetValue(value); } }
        public string PhoneNumberVal { get { return GetValue<string>(); } set { SetValue(value); } }
        #endregion

		public IEnumerable<State> States{get{return Mise.Core.ValueItems.State.GetUSStates ();}}
        #region commands

        public ICommand AddVendorCommand
        {
            get { return new SimpleCommand(AddVendor); }
        }

        #endregion

        async void AddVendor()
        {
            //make our address
            Processing = true;
            var address = MakeAddress();
            var phone = MakePhoneNumber();
            var email = MakeEmail();
            await _vendorService.AddVendor(Name, address, phone, email);
            Processing = false;
            //go back (pop it)
            await Navigation.CloseVendorAdd();
        }

        public StreetAddress MakeAddress()
        {
            if (
                (string.IsNullOrWhiteSpace(StreetAddressNumber) == false)
                && (string.IsNullOrWhiteSpace(StreetName) == false)
                && (string.IsNullOrWhiteSpace(City) == false)
                && (string.IsNullOrWhiteSpace(State) == false)
                && (string.IsNullOrWhiteSpace(Zip) == false))
            {
                return new StreetAddress(StreetAddressNumber, "", StreetName, City, State,
                    Country.UnitedStates.Name, Zip);
            }
            return null;
        }

        public PhoneNumber MakePhoneNumber()
        {
            if ((string.IsNullOrEmpty(PhoneAreaCode) == false)
               && (string.IsNullOrEmpty(PhoneNumberVal) == false)
                && PhoneNumber.IsValid(PhoneAreaCode, PhoneNumberVal)
            )
            {
                return new PhoneNumber(PhoneAreaCode, PhoneNumberVal);
            }
            return null;
        }

        public EmailAddress MakeEmail()
        {
            if (string.IsNullOrEmpty(Email) == false && EmailAddress.IsValid(Email))
            {
                return new EmailAddress(Email);
            }
            return null;
        }
    }
}

