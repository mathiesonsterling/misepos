using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
	public partial class UpdateQuantityPage : ContentPage
	{
		public UpdateQuantityPage ()
		{
			
			var vm = App.UpdateQuantityViewModel;
			Title = vm.Title;
			BindingContext = vm;

			InitializeComponent ();
		}

		protected override void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "UpdateQuantityPage"}});
		}
	}
}

