using System;
using System.Collections.Generic;
using Mise.Core.ValueItems;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;

namespace Mise.Inventory.Pages
{
	public partial class RestaurantSelectPage : ContentPage
	{
		public RestaurantSelectPage()
		{
			InitializeComponent();
		}
			
		protected override async void OnAppearing(){
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "RestaurantSelectPage"}});
		
			var vm = BindingContext as RestaurantSelectViewModel;
			if(vm != null){
				await vm.OnAppearing ();

				stckMain.Children.Clear ();

				var template = new DataTemplate (typeof(TextCell));
				template.SetBinding (TextCell.TextProperty, "FullName");
				var lv = new ListView {
					ItemsSource = vm.PossibleRestaurantNames,
					ItemTemplate = template
				};

				lv.ItemTapped += async (sender, e) => {
					var selName = e.Item as RestaurantName;
					((ListView)sender).SelectedItem = null;
					if(selName != null){
						await vm.SelectRestaurant(selName);
					}
				};
				stckMain.Children.Add (lv);
			}
		}
	}
}

