using Mise.Core.Entities.Inventory;

namespace MiseWebsite.Models
{
    public class PurchaseOrderLineItemViewModel : BaseBeverageLineItemViewModel
    {
        public decimal Quantity { get; set; }

        public PurchaseOrderLineItemViewModel(IPurchaseOrderLineItem li) : base(li)
        {
            Quantity = li.Quantity;
        }
    }
}
