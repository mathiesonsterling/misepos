using System;
using System.Collections.Generic;

using Xamarin.Forms;

using MisePOSTerminal;
using MisePOSTerminal.ViewModels;
using Mise.Core.Client.ApplicationModel;
using System.Reflection;

namespace MisePOSTerminal.Pages
{
	public partial class EmployeePage : ContentPage
	{
		readonly EmployeeViewModel _vm;

		public EmployeePage()
		{
			InitializeComponent();

			_vm = new EmployeeViewModel (App.Logger, App.AppModel);
			_vm.OnMoveToView += calling => App.MoveToPage (calling.DestinationView);
			BindingContext = _vm;
		}
	}
}

