using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomControls;

namespace StockboyForms.Pages
{
	public partial class ReceivingOrderPage : BasePage
	{
		ListView _customVL;
		public ReceivingOrderPage()
		{
			InitializeComponent();

			var vm = ViewModel as ReceivingOrderViewModel;
			vm.LoadItemsOnView = LoadItems;
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.ReceivingOrderViewModel;
			}
		}

		public override string PageName {
			get {
				return "ReceivingOrderPage";
			}
		}

		#endregion

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			stckNotes.IsVisible = Device.Idiom != TargetIdiom.Phone;
		}

		async Task LineItemDeleted(object item){
			var realItem = item as ReceivingOrderViewModel.ReceivingOrderDisplayLine;
			var vm = ViewModel as ReceivingOrderViewModel;
			if(realItem != null && vm != null){
				await vm.DeleteLineItem(realItem);
			}
		}

		void LoadItems ()
		{
			var vm = ViewModel as ReceivingOrderViewModel;
			if (vm != null) {
				if (_customVL == null) {
					_customVL = new ListView {
						ItemTemplate = new DataTemplate (() => new DeletableLineItemCell (LineItemDeleted)),
						RowHeight = 50,
						HasUnevenRows = true,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};
					_customVL.ItemTapped += async (sender, e) => {
						//mark it as the item being measured
						var lineItem = e.Item as ReceivingOrderViewModel.ReceivingOrderDisplayLine;
						if (lineItem != null) {
							await vm.SelectLineItem (lineItem);
						}
						((ListView)sender).SelectedItem = null;
					};
					lineItems.Children.Add (_customVL);
				}
				_customVL.ItemsSource = vm.LineItems;
				if(vm.FocusedItem != null){
					_customVL.ScrollTo(vm.FocusedItem, ScrollToPosition.MakeVisible, false);
				}
			}
		}
	}
}

