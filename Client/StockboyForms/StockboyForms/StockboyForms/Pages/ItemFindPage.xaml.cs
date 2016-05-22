using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;
using Mise.Inventory.CustomControls;


namespace StockboyForms.Pages
{
	public partial class ItemFindPage : BasePage
	{
		private ListView _lv;
		public ItemFindPage()
		{
			InitializeComponent();

			var vm = ViewModel as ItemFindViewModel;
			if(vm != null){
				vm.LoadItemsOnView = LoadItems;
			}
		}

		#region implemented abstract members of BasePage
		public override BaseViewModel ViewModel {
			get {
				return App.ItemFindViewModel;
			}
		}
		public override string PageName {
			get {
				return "ItemFindPage";
			}
		}
		#endregion

		void LoadItems(){
			var vm = ViewModel as ItemFindViewModel;
			if(vm != null){
				if (_lv == null) {
					//make our list view
					var template = new DataTemplate (typeof(TextCell));
					template.SetBinding (TextCell.TextProperty, "DisplayName");
					template.SetBinding (TextCell.DetailProperty, "DetailDisplay");
					_lv = new ListView {
						ItemTemplate = template,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};
					_lv.ItemSelected += async (sender, e) => {
						//fire the command
						var item = e.SelectedItem as IBaseBeverageLineItem;
						await vm.SelectLineItem (item);
					};
					stckPossibles.Children.Add (_lv);
				}
				_lv.ItemsSource = vm.LineItems;
			}
		}
	}
}

