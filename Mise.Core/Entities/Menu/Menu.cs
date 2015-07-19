using System.Collections.Generic;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Menu
{
	public class Menu : RestaurantEntityBase
    {
		public Menu(){
			DefaultMiseItems = new List<MenuItem> ();
		}

		public string DisplayName{ get; set;}
        public string Name { get; set; }

        /// <summary>
        /// This menu is active for consideration of rules
        /// </summary>
        public bool Active { get; set; }

		public IEnumerable<MenuItem> GetDefaultMiseItems(){
			return DefaultMiseItems;
		}
		public List<MenuItem> DefaultMiseItems {get;set;}

		public virtual IEnumerable<MenuItemCategory> GetMenuItemCategories (){
			return Categories;
		}

        public virtual List<MenuItemCategory> Categories { get; set; }

		public IEnumerable<MenuItem> GetAllMenuItems (){
			//get everything recusively
			var allItems = new List<MenuItem> ();
			allItems.AddRange (allItems);

			foreach (var cat in Categories) {
				allItems.AddRange (GetMenuItemsFromCatRecursively (cat));
			}

			return allItems;
		}

	    static IEnumerable<MenuItem> GetMenuItemsFromCatRecursively(MenuItemCategory cat){
			var list = new List<MenuItem> ();
			list.AddRange (cat.MenuItems);
			foreach (var subCat in cat.SubCategories) {
				list.AddRange (GetMenuItemsFromCatRecursively (subCat));
			}

			return list;
		}

    }
}
