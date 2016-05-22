using Mise.Inventory.ViewModels;
namespace StockboyForms.Pages.Reports
{
	public partial class ReportsPage : BasePage
	{
		public ReportsPage()
		{
			InitializeComponent();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.ReportsViewModel;
			}
		}

		public override string PageName {
			get {
				return "ReportsPage";
			}
		}

		#endregion

	}
}

