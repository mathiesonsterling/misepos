using System;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.Client.Services;
namespace Mise.Inventory.ViewModels
{
    public class AdminMenuViewModel : BaseViewModel
    {
        private ILoginService _loginService;
        public AdminMenuViewModel(ILogger logger, IAppNavigation navigationService, ILoginService loginService) 
            :base(navigationService, logger)
        {
            _loginService = loginService;

            PropertyChanged += (sender, e) => 
            {
                    if(e.PropertyName == "ReportingEmail"){
                        NewEmailIsValid = EmailAddress.IsValid(ReportingEmail);
                    }
            };
        }

        #region implemented abstract members of BaseViewModel

        public override async Task OnAppearing()
        {
            var rest = await _loginService.GetCurrentRestaurant();
            if (rest != null)
            {
                var email = rest.GetEmailsToSendInventoryReportsTo().FirstOrDefault();
                if (email != null)
                {
                    ReportingEmail = email.Value;
                }
            }

            if (string.IsNullOrEmpty(ReportingEmail))
            {
                var acct = await _loginService.GetCurrentAccountEmail();
                if (acct != null)
                {
                    ReportingEmail = acct.Value;
                }
            }
        }

        #endregion

        public string ReportingEmail{ get { return GetValue<string>(); } set { SetValue(value); } }

        public ICommand CancelAccountCommand{ get { return new Command(CancelAccount, () => _loginService.IsCurrentUserAccountOwner); } }
        public ICommand ChangeReportingEmailCommand{get{return new Command(ChangeReportingEmail, () => EmailAddress.IsValid(ReportingEmail));}}
        public bool NewEmailIsValid{ get { return GetValue<bool>(); } set { SetValue(value);} }

        private async void ChangeReportingEmail()
        {
            try{
                var email = new EmailAddress(ReportingEmail);
                await _loginService.ChangeCurrentRestaurantReportingEmail(email);

                await Navigation.ShowMainMenu();
            } catch(Exception e){
                HandleException(e);
            }
        }

        private async void CancelAccount()
        {
            var confirm = await AskUserQuestionModal("Cancel Subscription",
                              "Are you sure you want to cancel your subscription?  You'll be unable to use Stockboy until you reregister",
                              "Cancel Subscription", "No");
            if (confirm)
            {
                await _loginService.CancelAccount();

                await _loginService.LogOutAsync();

                await Navigation.ShowMainMenu();
            }
        }

    }
}

