using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomControls;
using Mise.Inventory.Services.Implementation;


namespace StockboyForms.Pages
{
	public partial class InventoryPage : BasePage
	{
		private DragListView _customVL;
		public InventoryPage()
		{
			InitializeComponent();
			var vm = ViewModel as InventoryViewModel;
			vm.LoadItemsOnView = LoadItems;

		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.InventoryViewModel;
			}
		}

		public override string PageName {
			get {
				return "InventoryPage";
			}
		}

		#endregion

		async Task LineItemDeleted(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = ViewModel as InventoryViewModel;
			if(realItem != null && vm != null){
				if (vm.CanDeleteLI (realItem)) {
					await vm.DeleteLineItem (realItem);
				}
			}
		}

		async Task LineItemMovedUp(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = ViewModel as InventoryViewModel;
			if(realItem != null && vm != null){
				await vm.MoveLineItemUp (realItem);
			}
		}	

		async Task LineItemMovedDown(object item){
			var realItem = item as InventoryViewModel.InventoryLineItemDisplayLine;
			var vm = ViewModel as InventoryViewModel;
			if(realItem != null && vm != null){
				await vm.MoveLineItemDown (realItem);
			}
		}

		void LoadItems ()
		{
			var vm = BindingContext as InventoryViewModel;
			if (vm != null) {

					var dataTemplate = new DataTemplate (() => new InventoryLineItemCell (LineItemDeleted, LineItemMovedUp, LineItemMovedDown));
				if (_customVL == null) {
					
					_customVL = new DragListView {
						ItemTemplate = dataTemplate,
						RowHeight = 50,
						HasUnevenRows = true,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};
					_customVL.ItemTapped += async (sender, e) => {
						//mark it as the item being measured
						var lineItem = e.Item as InventoryViewModel.InventoryLineItemDisplayLine;
						if (lineItem != null) {
							await vm.SelectLineItem (lineItem);
						}
						((ListView)sender).SelectedItem = null;
					};

					_customVL.ItemDraggedToNewPosition += async (sender, args) => {
						if(vm != null){
							await vm.MoveLineItemToPosition (args.OldIndex, args.NewIndex);
						}
					};
					listItems.Children.Add (_customVL);
				}
				_customVL.ItemsSource = vm.LineItems;
					if (vm.FocusedItem != null) {
						_customVL.ScrollTo (vm.FocusedItem, ScrollToPosition.MakeVisible, false);
					}

			}
		}
	}
}

