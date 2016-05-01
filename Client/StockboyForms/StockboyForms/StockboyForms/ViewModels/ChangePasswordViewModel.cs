using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

using Mise.Core.ValueItems;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Client.Services;
namespace Mise.Inventory.ViewModels
{
	public class ChangePasswordViewModel : BaseViewModel
	{
		private readonly ILoginService _loginService;
		public ChangePasswordViewModel (ILogger logger, IAppNavigation navi, ILoginService loginService) : base(navi, logger)
		{
			_loginService = loginService;

			PropertyChanged += (sender, e) => {
				FormValid = CanChange;
			};
		}

		#region implemented abstract members of BaseViewModel

		public override Task OnAppearing ()
		{
			return Task.FromResult (false);
		}

		#endregion

		public string OldPassword{ get { return GetValue<string> (); } set { SetValue (value); } }
		public string NewPassword{ get { return GetValue<string> (); } set { SetValue (value); } }
		public string NewPasswordConfirm{ get { return GetValue<string> (); } set { SetValue (value); } }

		public bool FormValid{ get { return GetValue<bool> (); } set { SetValue (value); } }

		public ICommand ChangePasswordCommand{get{return new Command (ChangePassword, () => CanChange);}}

		public async void ChangePassword(){
			try{
				if (!CanChange) {
					throw new ArgumentException ("Form is not valid");
				}
				Processing = true;
				var newPwd = new Password (NewPassword);
				var oldPwd = new Password(OldPassword);
				await _loginService.ChangePasswordForCurrentEmployee(oldPwd, newPwd);
				Processing = false;

				await Navigation.ShowMainMenu();
			} catch(Exception e){
				HandleException (e);
			}
		}

		private bool CanChange{
			get{return NotProcessing
				&& (!string.IsNullOrWhiteSpace (OldPassword))
				&& (!string.IsNullOrWhiteSpace (NewPassword))
				&& (!string.IsNullOrWhiteSpace (NewPasswordConfirm))
				&& NewPassword != OldPassword
				&& NewPassword == NewPasswordConfirm;
			}
		}
	}
}

