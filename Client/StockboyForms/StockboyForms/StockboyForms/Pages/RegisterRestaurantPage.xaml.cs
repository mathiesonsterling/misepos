using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin;
using Mise.Inventory.ViewModels;


namespace StockboyForms.Pages
{
	public partial class RegisterRestaurantPage : BasePage
	{
		public RegisterRestaurantPage ()
		{
			InitializeComponent ();

			pckState.SelectedIndexChanged += (sender, e) => {
                if(pckState != null && pckState.SelectedIndex >= 0)
                {
    				var selectedItem = pckState.Items[pckState.SelectedIndex];
    				var vm = ViewModel as RestaurantRegistrationViewModel;
    				vm.State = selectedItem;
                }
			};
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.RestaurantRegistrationViewModel;
			}
		}

		public override String PageName {
			get {
				return "RegisterRestaurantPage";
			}
		}

		#endregion

		protected override void OnAppearing(){
			base.OnAppearing ();
			var vm = ViewModel as RestaurantRegistrationViewModel;
			if(vm != null){
				pckState.Items.Clear ();
				foreach(var state in vm.States){
					pckState.Items.Add (state.Abbreviation);
				}
			}
		}
	}
}

