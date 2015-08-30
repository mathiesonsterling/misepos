using Mise.Core.Entities.Inventory;

namespace MiseReporting.Models
{
    public class ParLineItemViewModel : BaseBeverageLineItemViewModel
    {
        public decimal Quantity { get; set; }

        public ParLineItemViewModel()
        {
            
        }

        public ParLineItemViewModel(IParBeverageLineItem li) : base(li)
        {
            Quantity = li.Quantity;
        }
    }
}
