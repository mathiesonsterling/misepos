using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core;

namespace Mise.Inventory.Pages
{
	public partial class InvitationsPage : BasePage
	{
		public InvitationsPage ()
		{
			InitializeComponent ();
		}

		protected override void OnAppearing(){
			base.OnAppearing ();
			var vm = ViewModel as InvitationViewModel;
			vm.LoadDataOnView = LoadItems;
		}

		#region implemented abstract members of BasePage
		public override BaseViewModel ViewModel {
			get {
				return App.InvitationViewModel;
			}
		}
		public override String PageName {
			get {
				return "InvitationsPage";
			}
		}
		#endregion			

		private void LoadItems(){
			var vm = App.InvitationViewModel;
			foreach(var invite in vm.InvitesForUser){
				var container = new StackLayout();

				var label = new Label{ Text = invite.RestaurantName.ShortName};
				container.Children.Add (label);

				stckInvites.Children.Add (container);
			}
		}
	}
}

