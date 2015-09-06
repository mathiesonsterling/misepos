using System;
using System.Collections.Generic;
using Mise.Core.ValueItems;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;

namespace Mise.Inventory.Pages
{
	public partial class RestaurantSelectPage : BasePage
	{
		private ListView lv;
		public RestaurantSelectPage()
		{
			InitializeComponent();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.RestaurantSelectViewModel;
			}
		}

		public override string PageName {
			get {
				return "RestaurantSelectPage";
			}
		}

		#endregion
			
		protected override void OnAppearing(){
			base.OnAppearing();
			var vm = BindingContext as RestaurantSelectViewModel;
			if(vm != null){
				if(lv == null)
				{
					var template = new DataTemplate (typeof(TextCell));
					template.SetBinding (TextCell.TextProperty, "FullName");
					lv = new ListView {
						ItemTemplate = template,
						HorizontalOptions = LayoutOptions.FillAndExpand
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
				lv.ItemsSource = vm.PossibleRestaurantNames;
			}
		}
	}
}

