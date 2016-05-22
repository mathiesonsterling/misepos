using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;
using Mise.Inventory.ViewModels;
using Mise.Inventory.ViewModels.Reports;
namespace Mise.Inventory.Pages.Reports
{
	public partial class ReportsByInventoryPage : BasePage
	{
	    private Dictionary<string, IInventory> _possibleInventories;
		public ReportsByInventoryPage ()
		{
			InitializeComponent ();

		}

        private void LoadPickers(){
            Picker pickerStart;
            Picker pickerEnd;
            var vm = ViewModel as ReportsByInventoryViewModel;
            if (vm != null && stckStart != null && stckEnd != null)
            {
                stckStart.Children.Clear();
                pickerStart = new Picker{ Title = "Start Inventory" };
                pickerStart.SelectedIndexChanged += (sender, args) =>
                { 
                    if (vm != null)
                    {
                        vm.StartInventory = SelectItem(pickerStart);
                    }
                };
                stckStart.Children.Add(pickerStart);

            
                stckEnd.Children.Clear();
                pickerEnd = new Picker{ Title = "End Inventory" };
                pickerEnd.SelectedIndexChanged += (sender, e) => {
                    if(vm != null)
                    {
                        vm.EndInventory = SelectItem(pickerEnd);
                    }
                };
                stckEnd.Children.Add(pickerEnd);

                if (vm.CompletedInventories.Any())
                {
                    var possibleInvs = vm.CompletedInventories;
                    _possibleInventories = possibleInvs.Where(i => i.DateCompleted.HasValue)
                        .ToDictionary(i => i.DateCompleted.Value.LocalDateTime.ToString());

                    foreach (var p in _possibleInventories.Keys)
                    {
                        pickerStart.Items.Add(p);
                        pickerEnd.Items.Add(p);
                    }

                    if (pickerEnd.Items.Count > 1)
                    {
                        pickerEnd.SelectedIndex = pickerEnd.Items.Count - 1;
                        if (pickerStart.Items.Count > 2)
                        {
                            pickerStart.SelectedIndex = pickerStart.Items.Count - 2;
                        }
                    }
                }
            }
        }
           
	    private IInventory SelectItem(Picker picker)
	    {
            if (_possibleInventories != null)
            {
                var selectedString = picker.Items[picker.SelectedIndex];
                if (_possibleInventories.ContainsKey(selectedString))
                {
                    var selected = _possibleInventories[selectedString];
                    return selected;
                }
            }
	        return null;
	    }

		public override BaseViewModel ViewModel => App.ReportsByInventoryViewModel;

	    public override string PageName => "ReportsByInventoryPage";

	    protected override void OnAppearing()
	    {
			try{
		        base.OnAppearing();
                LoadPickers();
			} catch(Exception e)
			{
                var rep = e.Message;
			}
	    }
	}
}

