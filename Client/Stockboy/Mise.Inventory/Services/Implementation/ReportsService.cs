﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Reports;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems.Reports;

namespace Mise.Inventory.Services.Implementation
{
    public class ReportsService : IReportsService
    {
        private ReportRequest _currentRequest;
       
        private readonly ILogger _logger;
		private readonly IInventoryRepository _inventoryRepository;
		private readonly IReceivingOrderRepository _receivingOrderRepository;
        public ReportsService(ILogger logger, IInventoryRepository inventoryRepos, 
			IReceivingOrderRepository receivingOrderRepos)
        {
            _logger = logger;
			_inventoryRepository = inventoryRepos;
			_receivingOrderRepository = receivingOrderRepos;
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
                    return await report.RunReportAsync();
			case ReportTypes.AmountUsed:
				//get our starting inventory - the first one prior to start
				var startInventory = _inventoryRepository.GetAll ()
					.Where (i => i.DateCompleted.HasValue)
					.Where (i => i.DateCompleted.Value < _currentRequest.EndDate)
					.OrderByDescending (i => i.DateCompleted)
					.FirstOrDefault (i => i.DateCompleted <= _currentRequest.StartDate);
				var inventoriesInTime = _inventoryRepository.GetAll ()
					.Where (i => i.DateCompleted.HasValue)
					.Where (i => i.DateCompleted.Value >= _currentRequest.StartDate
				                        && i.DateCompleted.Value <= _currentRequest.EndDate);
				var recOrdersInTime = _receivingOrderRepository.GetAll ()
					.Where (ro => ro.DateReceived >= _currentRequest.StartDate
				                      && ro.DateReceived <= _currentRequest.EndDate
				                      );
				var amountReport = new AmountUsedInTimeReport (_currentRequest.StartDate.Value, _currentRequest.EndDate.Value, startInventory,
					                   recOrdersInTime, inventoriesInTime, _currentRequest.LiquidUnit);
				return await amountReport.RunReportAsync ();
                default:
                    throw new InvalidOperationException("Cant run report type " + _currentRequest.Type);
            }
        }
    }
}
