using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core;
using System.Runtime.InteropServices;
using Mise.Core.Entities.People;

namespace Mise.Inventory.Pages
{
	public partial class InvitationsPage : BasePage
	{
		private ListView _lv;
		public InvitationsPage ()
		{
			InitializeComponent ();
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
		/*
		private void LoadItemsNew(){
			var vm = ViewModel as InvitationViewModel;
			if (vm != null) {
				if (_lv == null) {
					var template = new DataTemplate (typeof(TextCell));
					template.SetBinding (TextCell.TextProperty, "RestaurantName");
					_lv = new ListView {
						ItemTemplate = template,
						HorizontalOptions = LayoutOptions.FillAndExpand
					};

					_lv.ItemTapped += async (sender, e) => {
						var selectedSection = e.Item as IApplicationInvitation;
						((ListView)sender).SelectedItem = null;
						if (selectedSection != null) {
							await vm.SelectLineItem (selectedSection);
						}
					};
					stckInvites.Children.Add (_lv);
				}
				_lv.ItemsSource = vm.InvitesForUser;
			}
		}*/
	}
}

