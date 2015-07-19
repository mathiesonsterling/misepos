using System;
using System.Collections.Generic;
using System.Reflection;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.POSTerminal.ViewModels;


namespace Mise.POSTerminal.Pages
{
	public partial class EmployeePage : ContentPage
	{
		readonly EmployeeViewModel _vm;

		public EmployeePage()
		{
			InitializeComponent();

			_vm = new EmployeeViewModel(App.Logger, App.AppModel);
//			_vm.OnMoveToView += calling => App.MoveToPage(calling.DestinationView);
			BindingContext = _vm;
		}
	}
}

