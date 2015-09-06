using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;

namespace Mise.Inventory.Pages
{
	public partial class PurchaseOrderSelectPage : BasePage
	{
		public PurchaseOrderSelectPage ()
		{
			var vm = ViewModel as PurchaseOrderSelectViewModel;
			vm.LoadItemsOnView = LoadItems;
			InitializeComponent ();
		}

		#region implemented abstract members of BasePage
		public override BaseViewModel ViewModel {
			get {
				return App.PurchaseOrderSelectViewModel;
			}
		}
		public override String PageName {
			get {
				return "PurchaseOrderSelectPage";
			}
		}
		#endregion			

		public void LoadItems(){
			var vm = BindingContext as PurchaseOrderSelectViewModel;
			stckPOs.Children.Clear ();
			var template = new DataTemplate (typeof(TextCell));
			template.SetBinding (TextCell.TextProperty, "DisplayName");
			template.SetBinding (TextCell.DetailProperty, "DetailDisplay");
			var lv = new ListView {
				ItemsSource = vm.LineItems,
				ItemTemplate = template,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			lv.ItemTapped += async (sender, e) => {
				var selectedPO = e.Item as PurchaseOrderLineDisplay;
				((ListView)sender).SelectedItem = null;
				if(selectedPO != null){
					await vm.SelectLineItem (selectedPO);
				}
			};
			stckPOs.Children.Add (lv);
		}
	}
}

