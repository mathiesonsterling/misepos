using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;
using Mise.Core.ExtensionMethods;
using Mise.Core.Services;
using Mise.Inventory.Services;
using Mise.Core.Repositories;
using Mise.Core.ValueItems.Reports;

namespace Mise.Inventory.ViewModels.Reports
{
    public class InventoryDisplayLine : ITextSearchable
    {
        private readonly IInventory _source;
        private readonly IEmployee _employee;
        public InventoryDisplayLine(IInventory source, IEmployee emp)
        {
            _source = source;
            _employee = emp;
        }

        public IInventory Source { get { return _source; } }
        public string DateCompleted
        {
            get
            {
                return
                    _source.DateCompleted.HasValue
					? _source.DateCompleted.Value.TimeAgo()
                    :"No date found";
            }
        }

        public string EmployeeName
        {
            get { return _employee != null ? _employee.DisplayName : string.Empty; }
        }

        public bool ContainsSearchString(string searchString)
        {
            return DateCompleted.ToUpper().Contains(searchString.ToUpper())
                   || EmployeeName.ToUpper().Contains(searchString.ToUpper());
        }
    }

    public class SelectCompletedInventoryViewModel : BaseSearchableViewModel<InventoryDisplayLine>
    {
        private readonly IInventoryService _inventoryService;
		private readonly IReportsService _reportsService;
        private readonly IEmployeeRepository _empRepos;
        public SelectCompletedInventoryViewModel(IAppNavigation navigation, ILogger logger, IInventoryService invService, 
            IEmployeeRepository empRepos, IReportsService reportsService)
            : base(navigation, logger)
        {
            _inventoryService = invService;
			_reportsService = reportsService;
            _empRepos = empRepos;
        }

        #region BaseSearchableViewModel methods
        public override async Task SelectLineItem(InventoryDisplayLine lineItem)
        {
            //set this as current
            if (lineItem != null)
            {
				Processing = true;
                //make the report, and send the request
				var inventoryId = lineItem.Source.ID;
				var currentRequest = await _reportsService.GetCurrentReportRequest ();
				if (currentRequest == null) {
					throw new InvalidOperationException ("No current request found");
				}
				var newRequest = new ReportRequest (ReportTypes.CompletedInventory, currentRequest.StartDate, currentRequest.EndDate, inventoryId, currentRequest.MaxResults);
				await _reportsService.SetCurrentReportRequest (newRequest);
				Processing = false;
                await Navigation.ShowReportResults();
            }
        }

        protected override async Task<ICollection<InventoryDisplayLine>> LoadItems()
        {
			var currentRequest = await _reportsService.GetCurrentReportRequest ();
			if (currentRequest == null) {
				throw new InvalidOperationException ("No current request found");
			}
            var inventories = await _inventoryService.GetCompletedInventoriesForCurrentRestaurant(currentRequest.StartDate, currentRequest.EndDate);

            //see if we can get an employee for each
            var invsAndEmps =
                inventories.Select(i => new {Inventory = i, Emp = _empRepos.GetByID(i.CreatedByEmployeeID)});

            var displayItems = invsAndEmps.Select(ie => new InventoryDisplayLine(ie.Inventory, ie.Emp));
			return displayItems.OrderByDescending (i => i.DateCompleted).ToList();
        }

        protected override void AfterSearchDone()
        {
        }
        #endregion
    }
}
