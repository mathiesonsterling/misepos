using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class MenuItemModifierGraphNode 
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public long Revision { get; set; }
        public Guid? RestaurantID { get; set; }

        public bool IsRequired { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PriceMultiplier { get; set; }
        public string Name { get; set; }

        public MenuItemModifierGraphNode()
        {
            
        }

        public MenuItemModifierGraphNode(MenuItemModifier source) 
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.OrderingID;

            IsRequired = source.IsRequired;
            PriceChange = source.PriceChange.Dollars;
            PriceMultiplier = source.PriceMultiplier;
            Name = source.Name;
        }

        public MenuItemModifier Rehydrate()
        {
            var mod = new MenuItemModifier
            {
                PriceChange = new Money(PriceChange),
                PriceMultiplier = PriceMultiplier,
                IsRequired = IsRequired,
                Name = Name,

                ID = ID,
                RestaurantID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = Revision}
            };

            return mod;
        }
    }
}
