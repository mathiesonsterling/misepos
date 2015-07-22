using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Inventory.Services;

namespace Mise.Inventory.ViewModels.Reports
{
    public class SelectCompletedInventoryViewModel : BaseSearchableViewModel<IInventory>
    {
        private readonly IInventoryService _inventoryService;
        public SelectCompletedInventoryViewModel(IAppNavigation navigation, ILogger logger, IInventoryService invService) 
            : base(navigation, logger)
        {
            _inventoryService = invService;
        }

        #region BaseSearchableViewModel methods
        public override async Task SelectLineItem(IInventory lineItem)
        {
            //set this as current
            if (lineItem != null)
            {
                //make the report, and send the request
                await Navigation.ShowReportResults();
            }
        }

        protected override async Task<ICollection<IInventory>> LoadItems()
        {
            var inventories = await _inventoryService.GetCompletedInventoriesForCurrentRestaurant();
            return inventories.ToList();
        }

        protected override void AfterSearchDone()
        {
        }
        #endregion
    }
}
