using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors;

namespace MiseVendorManagement.Models
{
    public class VendorItemForSaleViewModel : BaseBeverageLineItemViewModel
    {
        public Guid VendorId { get; set; }

        public decimal? PublicPrice { get; set; }
        public int? CaseSize { get; set; }

        public VendorItemForSaleViewModel() { }

        public IEnumerable<Guid> PostedCategoryGuids { get; set; }
        public IEnumerable<ICategory> PossibleCategories { get; set; }
        public IEnumerable<ICategory> SelectedCategories { get; set; }

        public Container Container { get; set; }
        public IEnumerable<Container> PossibleContainers { get; set; }
         
        public VendorItemForSaleViewModel(IVendorBeverageLineItem li) : base(li)
        {
            VendorId = li.VendorID;
            PublicPrice = li.PublicPricePerUnit?.Dollars;
            CaseSize = li.CaseSize;
            SelectedCategories = li.GetCategories();
        }
    }
}
