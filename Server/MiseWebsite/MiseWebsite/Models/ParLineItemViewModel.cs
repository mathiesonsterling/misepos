using Mise.Core.Entities.Inventory;

namespace MiseWebsite.Models
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
