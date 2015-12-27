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
            await _loginService.CancelAccount();

            await _loginService.LogOutAsync();

            await Navigation.ShowMainMenu();
        }

    }
}

