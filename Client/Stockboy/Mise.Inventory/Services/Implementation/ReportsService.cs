using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Services;
using Mise.Core.ValueItems.Reports;
using Mise.Inventory.Reports;
using Mise.Core.Repositories;
namespace Mise.Inventory.Services.Implementation
{
    public class ReportsService : IReportsService
    {
        private ReportRequest _currentRequest;
       
        private readonly ILogger _logger;
		private readonly IInventoryRepository _inventoryRepository;
        public ReportsService(ILogger logger, IInventoryRepository inventoryRepos)
        {
            _logger = logger;
			_inventoryRepository = inventoryRepos;
        }

        public Task SetCurrentReportRequest(ReportRequest request)
        {
            _currentRequest = request;
            return Task.FromResult(true);
        }

        public Task<ReportRequest> GetCurrentReportRequest()
        {
            return Task.FromResult(_currentRequest);
        }

        public async Task<ReportResult> RunCurrentReport()
        {
            if (_currentRequest == null)
            {
                throw new NotImplementedException("No request currently set to process!");
            }

            switch (_currentRequest.Type)
            {
			case ReportTypes.CompletedInventory:
					if (_currentRequest.EntityID.HasValue == false) {
						throw new InvalidOperationException ("ID is not set for which inventory to report on!");
					}
					var invToReport = _inventoryRepository.GetByID (_currentRequest.EntityID.Value);
                    var report = new CompletedInventoryReport(invToReport);
                    return report.RunReport();
                default:
                    throw new InvalidOperationException("Cant run report type " + _currentRequest.Type);
            }
        }
    }
}
