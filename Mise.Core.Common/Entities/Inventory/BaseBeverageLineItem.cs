using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
    public abstract class BaseBeverageLineItem : BaseTaggableRestaurantEntity, IBaseBeverageLineItem
    {
        public string MiseName { get; set; }

        string _displayName;
        public string DisplayName
        {
            get
            {
                var name = string.IsNullOrEmpty(_displayName) ? MiseName : _displayName;
                return name;
            }
            set
            {
                _displayName = value;
            }
        }

        public string UPC { get; set; }

        public LiquidContainer Container { get; set; }

        public int? CaseSize { get; set; }

        public virtual decimal Quantity { get; set; }

        public List<InventoryCategory> Categories { get; set; }

        public IEnumerable<ICategory> GetCategories()
        {
            return Categories;
        }

        public string CategoryDisplay => Categories.Any() ? Categories.First().Name : string.Empty;

        public virtual bool ContainsSearchString(string searchString)
        {
            return (string.IsNullOrEmpty(MiseName) == false && MiseName.ToUpper().Contains(searchString.ToUpper()))
|| (string.IsNullOrEmpty(DisplayName) == false && DisplayName.ToUpper().Contains(searchString.ToUpper()))
|| Quantity.ToString().Contains(searchString)
|| (Container != null && Container.ContainsSearchString(searchString))
    || (Categories != null && Categories.Any(c => c.ContainsSearchString(searchString)));
        }
    }
}
