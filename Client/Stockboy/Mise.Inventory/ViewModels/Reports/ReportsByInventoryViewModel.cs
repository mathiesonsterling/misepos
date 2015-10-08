using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Reports;
using Xamarin.Forms;
using Mise.Inventory.Services.Implementation;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Inventory.ViewModels.Reports
{
	public class ReportsByInventoryViewModel : ReportsViewModel
	{
		public ReportsByInventoryViewModel (IAppNavigation navi, ILogger logger, IInventoryService inventoryService,
		IReportsService reportsService, ILoginService loginService) 
			: base(navi, logger, reportsService, inventoryService, loginService)
		{
		}

		#region implemented abstract members of BaseViewModel

		public override async Task OnAppearing ()
		{
			await base.OnAppearing ();
			StartInventory = null;
			EndInventory = null;

			//load the inventories into the possibles
			CompletedInventories = await InventoryService.GetCompletedInventoriesForCurrentRestaurant (null, null);
		}

        #endregion

        public IInventory StartInventory
        {
            get { return GetValue<IInventory>(); }set{SetValue(value);}
        }

        public IInventory EndInventory
        {
            get{return GetValue<IInventory>();}set{SetValue(value);}
        }

	    protected override async Task DoGenericRequestFor(ReportTypes type)
	    {
            try
            {
                Processing = true;
                var unit = (LiquidAmountUnits)Enum.Parse(typeof(LiquidAmountUnits), LiquidUnit);

                var request = new ReportRequest(type, StartInventory, EndInventory, null, null, unit);
                await ReportsService.SetCurrentReportRequest(request);
                Processing = false;
                await Navigation.ShowReportResults();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

	    public IEnumerable<IInventory> CompletedInventories {
			get{ return GetValue<IEnumerable<IInventory>> (); }
			private set{ SetValue (value); }
		}
	}
}

