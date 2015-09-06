
using Mise.Inventory.ViewModels;

namespace Mise.Inventory.Pages
{
	public partial class MainMenuPage : BasePage
	{
		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.MainMenuViewModel;
			}
		}

		public override string PageName {
			get {
				return "MainMenuPage";
			}
		}

		#endregion

		public MainMenuPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			Icon = "mise.png";
			base.OnAppearing();
		}
	}
}

