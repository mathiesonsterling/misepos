using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory.Pages.Reports
{
	public partial class ReportsPage : ContentPage
	{
		public ReportsPage()
		{
			BindingContext = App.ReportsViewModel;
			InitializeComponent();
		}

		protected override void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "ReportsPage"}});
		}
	}
}

