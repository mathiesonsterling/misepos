using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
	public partial class EmployeesManagePage : ContentPage
	{
		public EmployeesManagePage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "EmployeesManagePage"}});
		}
	}
}

