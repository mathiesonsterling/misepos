using Mise.Core.Entities.Inventory;

namespace MiseWebsite.Models
{
    public class ReceivingOrderLineItemViewModel : BaseBeverageLineItemViewModel
    {
        public decimal? Price { get; set; }
        public decimal Quantity { get; set; }

        public ReceivingOrderLineItemViewModel(IReceivingOrderLineItem li) : base(li)
        {
            Price = li.LineItemPrice?.Dollars;
            Quantity = li.Quantity;
        }
    }
}
