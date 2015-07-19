using System.Collections.Generic;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Menu
{
    public interface IMenu : IEntityBase
    {
        string DisplayName { get; set; }
        string Name { get; set; }

        /// <summary>
        /// This menu is active for consideration of rules
        /// </summary>
        bool Active { get; set; }

        IEnumerable<MenuItem> GetDefaultMiseItems();
        IEnumerable<MenuItemCategory> GetMenuItemCategories();
        IEnumerable<MenuItem> GetAllMenuItems ();
    }
}