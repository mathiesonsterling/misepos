using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class MenuGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }

        public Guid RestaurantID { get; set; }

        public MenuGraphNode()
        {
            
        }

        public MenuGraphNode(Menu source) 
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            DisplayName = source.DisplayName;
            Name = source.Name;
            Active = source.Active;
        }

        public string DisplayName { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public Menu Rehydrate(IEnumerable<Guid> defaultMiseItemIDs, IEnumerable<MenuItemCategory> categories)
        {
            var menu = new Menu
            {
                ID = ID,
                RestaurantID = RestaurantID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision),
                DisplayName = DisplayName,
                Name = Name,
                Active = Active,
                //DefaultMiseItems = defaultMiseItems.ToList(),
                Categories = categories.ToList()
            };
            var defaults = menu.GetAllMenuItems().Where(mi => defaultMiseItemIDs.Contains(mi.ID)).ToList();
            menu.DefaultMiseItems = defaults;
            return menu;
        }
    }
}
