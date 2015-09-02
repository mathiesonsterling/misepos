
using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages
{
	public partial class AccountRegistrationPage : BasePage
	{
		public AccountRegistrationPage ()
		{
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.AccountRegistrationViewModel;
			}
		}

		public override string PageName {
			get {
				return "AccountRegistrationPage";
			}
		}

		#endregion

	}
}

