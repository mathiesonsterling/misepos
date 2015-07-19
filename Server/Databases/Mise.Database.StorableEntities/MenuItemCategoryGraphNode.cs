using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class MenuItemCategoryGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }
        public Guid RestaurantID { get; set; }

        public string Name { get; set; }
        public int DisplayOrder { get; set; }

        public MenuItemCategoryGraphNode()
        {
            
        }

        public MenuItemCategoryGraphNode(MenuItemCategory source)
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            Name = source.Name;
            DisplayOrder = source.DisplayOrder;
        }

        public MenuItemCategory Rehydrate(IEnumerable<MenuItem> menuItems, IEnumerable<MenuItemCategory> subCategories)
        {
            return new MenuItemCategory
            {
                ID = ID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                Name = Name,
                DisplayOrder = DisplayOrder,
                MenuItems = menuItems.ToList(),
                SubCategories = subCategories.ToList()
            };
        }
    }
}
