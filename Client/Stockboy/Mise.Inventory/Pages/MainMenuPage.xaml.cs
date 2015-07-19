using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;

namespace Mise.Inventory.Pages
{
	public partial class MainMenuPage : ContentPage
	{
		public MainMenuPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "MainMenuPage"}});
			base.OnAppearing();

			this.Icon = "mise.png";
			var vm = BindingContext as MainMenuViewModel;
			if(vm != null){
				await vm.OnAppearing ();
			}
		}
	}
}

