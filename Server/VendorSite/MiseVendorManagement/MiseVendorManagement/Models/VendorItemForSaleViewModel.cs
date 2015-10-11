using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public IEnumerable<ICategory> PossibleCategories { get; set; }

        [NotMapped]
        public IEnumerable<ICategory> SelectedCategories { get; set; }

        [NotMapped]
        public Container Container { get; set; }

        [NotMapped]
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
