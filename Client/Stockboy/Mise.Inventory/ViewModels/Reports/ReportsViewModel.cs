using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Services;
using Mise.Core.ValueItems.Reports;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;

namespace Mise.Inventory.ViewModels.Reports
{
	public class ReportsViewModel : BaseViewModel
	{
	    private readonly IReportsService _reportsService;
		private readonly ILogger _logger;
		public ReportsViewModel(IAppNavigation navigation, ILogger logger, IReportsService reportsService) : base(navigation, logger)
		{
			_logger = logger;
		    _reportsService = reportsService;
		    StartDate = new DateTime(2015, 1, 1);
			EndDate = DateTime.Now.AddDays(1);
		}

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }

        #region Fields
        public DateTime StartDate { get { return GetValue<DateTime>(); } set { SetValue(value);} }
        public DateTime EndDate { get { return GetValue<DateTime>(); } set { SetValue(value);} }
        #endregion

        #region Commands
		public ICommand CompletedInventoriesCommand { 
			get { return new SimpleCommand(CompletedInventories, () => NotProcessing);}
		}

	    private async void CompletedInventories()
	    {
	        try
	        {
	            Processing = true;
                //set our request to limit dates
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

	    #endregion
	}
}

