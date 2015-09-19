using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.ViewModels.Reports;
namespace Mise.Inventory.Pages.Reports
{
	public partial class ReportsByInventoryPage
	{
	    private Dictionary<string, IInventory> _possibleInventories;
		public ReportsByInventoryPage ()
		{
			InitializeComponent ();
            pickerStart.SelectedIndexChanged += (sender, args) =>
            {
                
            }
		}

		public override BaseViewModel ViewModel => App.ReportsByInventoryViewModel;

	    public override string PageName => "ReportsByInventoryPage";

	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
	        var vm = ViewModel as ReportsByInventoryViewModel;
	        if (vm != null)
	        {
                var possibleInvs = vm.CompletedInventories;
	            _possibleInventories = possibleInvs.ToDictionary(i => i.DateCompleted.ToString());

                pickerStart.Items.Clear();
                pickerEnd.Items.Clear();
	            foreach (var p in _possibleInventories.Keys)
	            {
	                pickerStart.Items.Add(p);
	                pickerEnd.Items.Add(p);
	            }
	        }
	    }
	}
}

