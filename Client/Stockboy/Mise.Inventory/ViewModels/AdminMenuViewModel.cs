using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
namespace Mise.Inventory.ViewModels
{
    public class AdminMenuViewModel : BaseViewModel
    {
        private ILoginService _loginService;
        public AdminMenuViewModel(ILogger logger, IAppNavigation navigationService, ILoginService loginService) 
            :base(navigationService, logger)
        {
            _loginService = loginService;
        }

        #region implemented abstract members of BaseViewModel

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }

        #endregion

        public ICommand CancelAccountCommand{ get { return new Command(CancelAccount, () => _loginService.IsCurrentUserAccountOwner); } }

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

