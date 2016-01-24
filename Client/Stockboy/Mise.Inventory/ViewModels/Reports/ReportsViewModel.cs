using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems.Reports;

using Mise.Inventory.Services;
using Mise.Core.ValueItems.Inventory;
using Xamarin.Forms;
using Mise.Core.Client.Services;
namespace Mise.Inventory.ViewModels.Reports
{
	public class ReportsViewModel : BaseViewModel
	{
	    protected readonly IReportsService ReportsService;
		protected readonly IInventoryService InventoryService;
		private readonly ILoginService _loginService;
		public ReportsViewModel(IAppNavigation navigation, ILogger logger, 
			IReportsService reportsService, IInventoryService inventoryService, ILoginService loginService) : base(navigation, logger)
		{
			_loginService = loginService;
		    ReportsService = reportsService;
			InventoryService = inventoryService;
		    StartDate = new DateTime(2015, 1, 1);
			EndDate = DateTime.Now.AddDays(1);
			LiquidUnit = LiquidAmountUnits.Milliliters.ToString ();

            PropertyChanged += (sender, e) => {
                if(e.PropertyName == "StartDate" || e.PropertyName == "EndDate" || e.PropertyName == "NotProcessing"){
                    CanReport = NotProcessing && (StartDate != null) && (EndDate != null);
                }
            };
		}

        public override async Task OnAppearing()
        {
            //do we have a first item
			Processing = true;
			var firstInventory = await InventoryService.GetFirstCompletedInventory ();
			if (firstInventory != null) {
				StartDate = firstInventory.DateCompleted.Value.AddMinutes (1).LocalDateTime;
				if (StartDate.AddDays(1) > EndDate) {
					EndDate = StartDate.AddDays (1);
				}
			}
			Processing = false;
        }

        #region Fields
        public DateTime StartDate { get { return GetValue<DateTime>(); } set { SetValue(value);} }
        public DateTime EndDate { get { return GetValue<DateTime>(); } set { SetValue(value);} }

        public string LiquidUnit{get{return GetValue<string> ();}set{ SetValue (value); }}

        public bool CanReport{ get { return GetValue<bool>(); } set { SetValue(value); } }
        #endregion

        #region Commands
		public ICommand CompletedInventoriesCommand { 
			get { return new Command(CompletedInventories, () => NotProcessing);}
		}

		public ICommand AmountUsedCommand{
			get{return new Command (AmountUsed);}
		}

		public ICommand CostOfGoodsSoldCommand{
			get{return new Command(CostOfGoodsSold);}
		}

	    private async void CompletedInventories()
	    {
	        try
	        {
	            Processing = true;
                //set our request to limit dates
				var unit = (LiquidAmountUnits)Enum.Parse (typeof(LiquidAmountUnits), LiquidUnit);
	            var request = new ReportRequest(ReportTypes.CompletedInventory, StartDate, EndDate, null, null, unit);
	            await ReportsService.SetCurrentReportRequest(request);
	            Processing = false;
                await Navigation.ShowSelectCompletedInventory();
	        }
	        catch (Exception e)
	        {
	            HandleException(e);
	        }
	    }

		private async void AmountUsed(){
			var rest = await _loginService.GetCurrentRestaurant ();
			if(rest == null){
				throw new InvalidOperationException ("Cannot do report without a selected restaurant");
			}
			var hasPriorInv = await InventoryService.HasInventoryPriorToDate (rest.Id, StartDate);
			if(hasPriorInv == false){
				var userRes = await AskUserQuestionModal("No prior inventory", 
					              "There's not a starting inventory before the dates you selected.  Your results will only depend on items you received, and might not be accurrate.", "Continue");
				if(userRes == false){
					return;
				}
			}
			await DoGenericRequestFor (ReportTypes.AmountUsed);
		}

		private async void CostOfGoodsSold(){
			await DoGenericRequestFor (ReportTypes.COGS);
		}
	    #endregion

		protected virtual async Task DoGenericRequestFor(ReportTypes type){
			try{
				Processing = true;
				var unit = (LiquidAmountUnits)Enum.Parse (typeof(LiquidAmountUnits), LiquidUnit);
		
				var request = new ReportRequest (type, StartDate, EndDate, null, null, unit);
				await ReportsService.SetCurrentReportRequest (request);
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

