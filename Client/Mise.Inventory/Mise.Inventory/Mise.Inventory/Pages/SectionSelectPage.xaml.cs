using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;

namespace Mise.Inventory.Pages
{
	public partial class SectionSelectPage : BasePage
	{
		private ListView _lv;
		public SectionSelectPage()
		{
			InitializeComponent();
		}

		#region implemented abstract members of BasePage

		public override BaseViewModel ViewModel {
			get {
				return App.SectionSelectViewModel;
			}
		}

		public override String PageName {
			get {
				return "SectionSelectPage";
			}
		}

		#endregion

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var vm = ViewModel as SectionSelectViewModel;

			if (_lv == null) {
				slOther.Children.Clear ();
				var template = new DataTemplate (typeof(TextCell));
				template.SetBinding (TextCell.TextProperty, "Name");
				_lv = new ListView {
					ItemTemplate = template,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};

				_lv.ItemTapped += async (sender, e) => {
					var selectedSection = e.Item as IInventorySection;
					((ListView)sender).SelectedItem = null;
					if (selectedSection != null) {
						await vm.SelectLineItem (selectedSection);
					}
				};
				slOther.Children.Add (_lv);
			}
			_lv.ItemsSource = vm.LineItems;
		}
	}
}

