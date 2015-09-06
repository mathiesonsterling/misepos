using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Inventory.ViewModels;
namespace Mise.Inventory.Pages
{
	public partial class VendorAddPage : BasePage
	{
		public VendorAddPage()
		{
			InitializeComponent();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.VendorAddViewModel;
			}
		}

		public override string PageName {
			get {
				return "VendorAddPage";
			}
		}

		#endregion

		protected override void OnAppearing(){
			var vm = ViewModel as VendorAddViewModel;
			if (vm != null) {
				pckState.Items.Clear ();
				foreach (var state in vm.States) {
					pckState.Items.Add (state.Abbreviation);
				}

				pckState.SelectedIndexChanged += (sender, e) => {
					var selectedItem = pckState.Items [pckState.SelectedIndex];
					vm.State = selectedItem;
				};
			}
		}
	}
}

