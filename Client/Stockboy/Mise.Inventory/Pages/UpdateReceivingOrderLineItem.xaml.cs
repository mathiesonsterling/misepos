using System;
using System.Collections.Generic;
using Mise.Inventory.ViewModels;
using Xamarin.Forms;

namespace Mise.Inventory
{
	public partial class UpdateReceivingOrderLineItem : ContentPage
	{
		public UpdateReceivingOrderLineItem ()
		{
		    BindingContext = App.UpdateReceivingOrderLineItemViewModel;
			InitializeComponent ();
		}

	    protected override async void OnAppearing()
	    {
	        var vm = BindingContext as UpdateReceivingOrderLineItemViewModel;
	        if (vm != null)
	        {
	            await vm.OnAppearing();
				//btnNext.IsEnabled = vm.MoveNextCommand.CanExecute (null);
	        }
	    }
	}
}

