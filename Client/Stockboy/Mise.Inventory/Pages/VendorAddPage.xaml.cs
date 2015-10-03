using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Inventory.ViewModels;
using Mise.Core.ValueItems;


namespace Mise.Inventory.Pages
{
	public partial class VendorAddPage : BasePage
	{
		public VendorAddPage()
		{
			InitializeComponent();

			var vm = ViewModel as VendorAddViewModel;
			pckState.SelectedIndexChanged += (sender, e) => {
				var selectedItem = pckState.Items [pckState.SelectedIndex];
				vm.State = selectedItem;
			};
			vm.SelectStateOnView = SelectState;
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

		protected override async void OnAppearing(){
			var vm = ViewModel as VendorAddViewModel;
			if (vm != null) {
				pckState.Items.Clear ();
				foreach (var state in vm.States) {
					pckState.Items.Add (state.Abbreviation);
				}
			}
			await vm.OnAppearing ();
		}

		public void SelectState(State state){
			if (state == null) {
				return;
			}
			if (pckState.Items.Contains (state.Abbreviation)) {
				pckState.SelectedIndex = pckState.Items.IndexOf (state.Abbreviation);
			}
		}
	}
}

