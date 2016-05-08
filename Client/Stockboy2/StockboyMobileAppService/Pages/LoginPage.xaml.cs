using Mise.Inventory.ViewModels;
using Mise.Core.ValueItems;
namespace Mise.Inventory.Pages
{
	public partial class LoginPage : BasePage
	{
		public LoginPage()
		{
			InitializeComponent();

			entEmail.Completed += (sender, e) => {
				var vm = ViewModel as LoginViewModel;
				if(vm != null){
					if(string.IsNullOrEmpty (vm.Username) == false){
						if(EmailAddress.IsValid (vm.Username))
						{
							if(string.IsNullOrEmpty (vm.Password) == false){
								vm.LoginCommand.Execute (null);
							}
							else {
								entPassword.Focus ();
							}
						} else {
							vm.Username = string.Empty;
							entEmail.Focus ();
						}
					}
				}
			};

			entPassword.Completed += (sender, e) => {
				var vm = ViewModel as LoginViewModel;
				if(vm != null){
					if(string.IsNullOrEmpty (vm.Password) == false){
						if(string.IsNullOrEmpty (vm.Username) == false){
							vm.LoginCommand.Execute (null);
						}
						else {
							entEmail.Focus ();
						}
					}
				}
			};
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.LoginViewModel;
			}
		}

		public override string PageName {
			get {
				return "LoginPage";
			}
		}

		#endregion
	}
}

