using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.ExtensionMethods;
using Mise.Inventory.Services;
using Mise.Inventory.MVVM;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public class PurchaseOrderLineDisplay : BaseDisplayLine<IPurchaseOrder>
	{
		public PurchaseOrderLineDisplay(IPurchaseOrder source) : base(source){
		}
		public override string DisplayName {
			get {
				return Source.CreatedDate.TimeAgo ();
			}
		}

		public override string DetailDisplay {
			get {
				return "Sent by " + Source.CreatedByName;
			}
		}
	}

	public class PurchaseOrderSelectViewModel : BaseSearchableViewModel<PurchaseOrderLineDisplay>
    {


        private readonly IPurchaseOrderService _poService;
        readonly IVendorService _vendorService;
        readonly IReceivingOrderService _roService;
        private readonly IInsightsService _insights;
        
        /// <summary>
        /// Whether we skipped this item on the way over
        /// </summary>
        public PurchaseOrderSelectViewModel(IAppNavigation appNav, IPurchaseOrderService poService,
            IVendorService vendorService, IReceivingOrderService roService, ILogger logger, IInsightsService insightsService)
            : base(appNav, logger)
        {
            _poService = poService;
            _roService = roService;
            _insights = insightsService;
            _vendorService = vendorService;
        }

        public ICommand StartBlankReceivingOrderCommand { 
			get { return new SimpleCommand(StartBlankReceivingOrder, () => NotProcessing); } 
		}


        #region implemented abstract members of BaseSearchableViewModel
        public override async Task SelectLineItem(PurchaseOrderLineDisplay lineItem)
        {
            try
            {
                //have the RO created from this PO
                using (_insights.TrackTime("TimeToGeneratePO"))
                {
                    await _roService.StartReceivingOrder(lineItem.Source);
                }

                await Navigation.ShowReceivingOrder();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        protected override async Task<ICollection<PurchaseOrderLineDisplay>> LoadItems()
        {
            var vendor = await _vendorService.GetSelectedVendor();
            var items = await _poService.GetPurchaseOrdersWaitingForVendor(vendor);

			return items.OrderBy(po => po.CreatedDate).Select(po => new PurchaseOrderLineDisplay (po)).ToList();
        }


        protected override void AfterSearchDone()
        {
        }
        #endregion

        private async void StartBlankReceivingOrder()
        {
            try
            {
                await _roService.StartReceivingOrderForSelectedVendor();

                await Navigation.ShowReceivingOrder();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }
    }
}

