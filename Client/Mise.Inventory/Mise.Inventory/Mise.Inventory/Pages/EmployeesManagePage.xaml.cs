
using System;

namespace Mise.Inventory.Pages
{
	public partial class EmployeesManagePage : BasePage
	{
		public EmployeesManagePage()
		{
			InitializeComponent();
		}

		#region implemented abstract members of BasePage

		public override Mise.Inventory.ViewModels.BaseViewModel ViewModel {
			get {
				return App.EmployeesManageViewModel;
			}
		}

		public override String PageName {
			get {
				return "EmployeesManagePage";
			}
		}

		#endregion
	}
}

