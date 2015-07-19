using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class MenuItemModifierGroupGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }
        public Guid? RestaurantID { get; set; }

        public string DisplayName { get; set; }
        public bool Exclusive { get; set; }
        public bool Required { get; set; }
        public Guid? DefaultItemID { get; set; }

        public MenuItemModifierGroupGraphNode() { }

        public MenuItemModifierGroupGraphNode(MenuItemModifierGroup modGroup) 
        {
            ID = modGroup.ID;
            RestaurantID = modGroup.RestaurantID;
            CreatedDate =modGroup.CreatedDate;
            LastUpdatedDate = modGroup.LastUpdatedDate;
            Revision = modGroup.Revision.ToDatabaseString();

            DisplayName = modGroup.DisplayName;
            Exclusive = modGroup.Exclusive;
            Required = modGroup.Required;
            DefaultItemID = modGroup.DefaultItemID;
        }

        public MenuItemModifierGroup Rehydrate(IEnumerable<MenuItemModifier> mods)
        {
            var modG = new MenuItemModifierGroup
            {
                DisplayName = DisplayName,
                Exclusive = Exclusive,
                Required = Required,
                DefaultItemID = DefaultItemID,
                Modifiers = mods.ToList(),

                ID = ID,
                RestaurantID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision)
            };

            return modG;
        }

        public bool Equals(IEntityBase other)
        {
            return ID.Equals(other.ID) && Revision.Equals(other.Revision.ToDatabaseString());
        }
    }
}
