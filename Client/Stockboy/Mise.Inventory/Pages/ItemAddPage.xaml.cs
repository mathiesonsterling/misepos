using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using XLabs.Forms.Mvvm;

namespace Mise.Inventory.Pages
{
	public partial class ItemAddPage : BasePage
	{
		public ItemAddPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var vm = ViewModel as ItemAddViewModel;
			if(vm != null){
				LoadPickers ();
			}
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.ItemAddViewModel;
			}
		}

		public override String PageName {
			get {
				return "ItemAddPage";
			}
		}

		#endregion

		public void LoadPickers(){
			//get the picker items
			var vm = ViewModel as ItemAddViewModel;
			if(vm != null){
				pckContainer.Items.Clear ();
				foreach(var opt in vm.PossibleContainerNames){
					pckContainer.Items.Add (opt);
				}

				pckContainer.SelectedIndexChanged += (sender, e) => {
					var selected = pckContainer.Items[pckContainer.SelectedIndex];
					vm.SelectedContainerName = selected;
				};

				pckCategory.Items.Clear ();
				foreach(var cat in vm.PossibleCategoryNames){
					pckCategory.Items.Add (cat);
				}

				pckCategory.SelectedIndexChanged += (sender, e) => {
					var selected = pckCategory.Items[pckCategory.SelectedIndex];
					vm.SelectedCategoryName = selected;
				};
			}
				
		}
	}
}

