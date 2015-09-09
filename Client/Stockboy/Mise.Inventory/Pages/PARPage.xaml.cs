using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.CustomControls;
namespace Mise.Inventory.Pages
{
	public partial class PARPage : BasePage
	{
		ListView _customVL;
		public PARPage()
		{
			InitializeComponent();

			var vm = BindingContext as ParViewModel;
			vm.LoadItemsOnView = LoadItems;

		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.ParViewModel;
			}
		}

		public override String PageName {
			get {
				return "ParPage";
			}
		}

		#endregion

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var vm = ViewModel as ParViewModel;
		    if (vm != null)
			{
				try{
					if(vm.FocusedItem != null && vm.LineItems.Any() && vm.LineItems.Contains(vm.FocusedItem)){
						if(_customVL != null){
							_customVL.ScrollTo (vm.FocusedItem, ScrollToPosition.MakeVisible, false);
						}
					}
				} catch(Exception e){
					Xamarin.Insights.Report (e);
				}
		    }
		}

		async Task LineItemDeleted(object item){
			var realItem = item as ParViewModel.ParLineItemDisplay;
			var vm = ViewModel as ParViewModel;
			if(realItem != null && vm != null){
				if(vm.CanDeleteLineItem (realItem)){
					await vm.DeleteLineItem(realItem);
				}
			}
		}

		void LoadItems ()
		{
			var vm = ViewModel as ParViewModel;
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
						var lineItem = e.Item as ParViewModel.ParLineItemDisplay;
						if (lineItem != null) {
							await vm.SelectLineItem (lineItem);
						}
						((ListView)sender).SelectedItem = null;
					};
					lineItems.Children.Add (_customVL);
				}
				_customVL.ItemsSource = vm.LineItems;
			}
		}
	}
}

