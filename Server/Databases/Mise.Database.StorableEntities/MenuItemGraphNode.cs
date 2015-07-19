using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities
{
    public sealed class MenuItemGraphNode : IStorableEntityGraphNode
    {
        public Guid ID { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }
        public Guid? RestaurantID { get; set; }

        public string Name { get; set; }
        public int DisplayWeight { get; set; }
        public string OIListName { get; set; }
        public string ButtonName { get; set; }
        public string PrinterName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// CSV string of all our destinations
        /// </summary>
        public string Destinations { get; set; }

        public MenuItemGraphNode()
        {
            
        }

        public MenuItemGraphNode(MenuItem source) 
        {
            ID = source.ID;
            RestaurantID = source.RestaurantID;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();

            Name = source.Name;
            ButtonName = source.ButtonName;
            OIListName = source.OIListName;
            PrinterName = source.PrinterName;
            Price = source.Price.Dollars;
            Description = source.Description;
            Destinations = string.Join(",", source.Destinations.Select(d => d.Name));
        }

        public MenuItem Rehydrate(IEnumerable<MenuItemModifierGroup> mods)
        {
            var dests = Destinations.Split(new[] {','}).Select(d => new OrderDestination{Name = d}).ToList();
            var mi = new MenuItem
            {
                Name = Name,
                ButtonName = ButtonName,
                OIListName = OIListName,
                PrinterName = PrinterName,
                Price = new Money(Price),
                Description = Description,
                PossibleModifiers = mods.ToList(),
                Destinations = dests,

                ID = ID,
                RestaurantID = ID,
                CreatedDate = CreatedDate,
                LastUpdatedDate = LastUpdatedDate,
                Revision = new EventID(Revision)
            };

            return mi;
        }
    }
}
