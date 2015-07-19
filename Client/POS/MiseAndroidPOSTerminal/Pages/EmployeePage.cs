using Android.App;

using MiseAndroidPOSTerminal.AndroidViews;

using Mise.Core.Client.ApplicationModel;

namespace MiseAndroidPOSTerminal
{
	[Activity(Label = "Employee")]			
	public class EmployeePage : BaseXamarinFormsHostPage
	{
		#region implemented abstract members of BaseXamarinFormsHostPage

		protected override TerminalViewTypes Type {
			get {
				return TerminalViewTypes.EmployeePage;
			}
		}

		#endregion
	}
}
