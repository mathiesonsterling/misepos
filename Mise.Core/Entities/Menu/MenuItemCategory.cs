using System.Collections.Generic;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Menu
{
    public class MenuItemCategory : RestaurantEntityBase
    {
        public MenuItemCategory()
        {
            SubCategories = new List<MenuItemCategory>();
            MenuItems = new List<MenuItem>();
        }

        public string Name
        {
            get;
            set;
        }
			
        public int DisplayOrder
        {
            get;
            set;
        }

        public List<MenuItem> MenuItems { get; set; }

        public List<MenuItemCategory> SubCategories { get; set; }
    }
}
