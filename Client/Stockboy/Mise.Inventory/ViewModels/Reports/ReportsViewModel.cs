using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Services;
using Mise.Core.ValueItems.Reports;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Inventory.ViewModels.Reports
{
	public class ReportsViewModel : BaseViewModel
	{
	    private readonly IReportsService _reportsService;
		private readonly IInventoryService _inventoryService;
		private readonly ILogger _logger;
		public ReportsViewModel(IAppNavigation navigation, ILogger logger, 
			IReportsService reportsService, IInventoryService inventoryService) : base(navigation, logger)
		{
			_logger = logger;
		    _reportsService = reportsService;
			_inventoryService = inventoryService;
		    StartDate = new DateTime(2015, 1, 1);
			EndDate = DateTime.Now.AddDays(1);
			LiquidUnit = LiquidAmountUnits.Milliliters.ToString ();
		}

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }

        #region Fields
        public DateTime StartDate { get { return GetValue<DateTime>(); } set { SetValue(value);} }
        public DateTime EndDate { get { return GetValue<DateTime>(); } set { SetValue(value);} }
		public string LiquidUnit{get{return GetValue<string> ();}set{ SetValue (value); }}
        #endregion

        #region Commands
		public ICommand CompletedInventoriesCommand { 
			get { return new SimpleCommand(CompletedInventories, () => NotProcessing);}
		}

		public ICommand AmountUsedCommand{
			get{return new SimpleCommand (AmountUsed, () => NotProcessing);}
		}

	    private async void CompletedInventories()
	    {
	        try
	        {
	            Processing = true;
                //set our request to limit dates
				var unit = (LiquidAmountUnits)Enum.Parse (typeof(LiquidAmountUnits), LiquidUnit);
	            var request = new ReportRequest(ReportTypes.CompletedInventory, StartDate, EndDate, null, null);
	            await _reportsService.SetCurrentReportRequest(request);
	            Processing = false;
                await Navigation.ShowSelectCompletedInventory();
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	        }
	    }

		private async void AmountUsed(){
			var hasPriorInv = await _inventoryService.HasInventoryPriorToDate (StartDate);
			if(hasPriorInv == false){
				var userRes = await Navigation.AskUser ("No prior inventory", 
					              "There's not a starting inventory before the dates you selected.  Your results will only depend on items you received, and might not be accurrate.  Continue?");
				if(userRes == false){
					return;
				}
			}
			await DoGenericRequestFor (ReportTypes.AmountUsed);
		}

	    #endregion

		private async Task DoGenericRequestFor(ReportTypes type){
			try{
				Processing = true;
				var unit = (LiquidAmountUnits)Enum.Parse (typeof(LiquidAmountUnits), LiquidUnit);
		
				var request = new ReportRequest (type, StartDate, EndDate, null, null, unit);
				await _reportsService.SetCurrentReportRequest (request);
				Processing = false;
				await Navigation.ShowReportResults ();
			}
			catch (Exception e)
			{
				HandleException(e);
			}
		}
	}
}

