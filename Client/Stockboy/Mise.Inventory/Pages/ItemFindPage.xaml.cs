using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;
namespace Mise.Inventory.Pages
{
	public partial class ItemFindPage : ContentPage
	{
		public ItemFindPage()
		{
			InitializeComponent();

			var vm = BindingContext as ItemFindViewModel;
			if(vm != null){
				vm.LoadItemsOnView = LoadItems;
			}
		}

		protected override async void OnAppearing ()
		{
			Xamarin.Insights.Track("ScreenLoaded", new Dictionary<string, string>{{"ScreenName", "ItemFindPage"}});
			var vm = BindingContext as ItemFindViewModel;
		    if (vm != null)
		    {
		        await vm.OnAppearing();
		    }
		}

		void LoadItems(){
			var vm = BindingContext as ItemFindViewModel;
			if(vm != null){
				stckPossibles.Children.Clear ();

				//make our list view
				var template = new DataTemplate (typeof(TextCell));
				template.SetBinding (TextCell.TextProperty, "DisplayName");
				template.SetBinding (TextCell.DetailProperty, "DetailDisplay");
				var lv = new ListView {
					ItemsSource = vm.LineItems,
					ItemTemplate = template,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				lv.ItemSelected += async (sender, e) => {
					//fire the command
					var item = e.SelectedItem as IBaseBeverageLineItem;
					await vm.SelectLineItem(item);
				};
				stckPossibles.Children.Add(lv);
			}
		}
	}
}

