using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mise.Inventory.Pages
{
	public partial class SectionAddPage : BasePage
	{
		public SectionAddPage()
		{
			InitializeComponent();
		}

		#region implemented abstract members of BasePage

		public override Mise.Inventory.ViewModels.BaseViewModel ViewModel {
			get {
				return App.SectionAddViewModel;
			}
		}

		public override string PageName {
			get {
				return "SectionAddPage";
			}
		}

		#endregion
	}
}

