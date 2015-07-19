using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.POSTerminal.ViewModels;

namespace Mise.POSTerminal.Pages
{
	public partial class SplashPage : ContentPage
	{
		readonly SplashPageViewModel _vm;

		public SplashPage()
		{
			InitializeComponent();

			_vm = new SplashPageViewModel(App.Logger, App.AppModel, App.Navigation);
			BindingContext = _vm;
		}
	}
}

