﻿using Mise.Core.Entities.Inventory;

namespace Mise.Core.Client.Entities.Inventory
{
    public class RestaurantInventorySection 
        : BaseDbEntity<IRestaurantInventorySection, Core.Common.Entities.Inventory.RestaurantInventorySection>
    {
	    public RestaurantInventorySection()
	    {

	    }

	    public RestaurantInventorySection(IRestaurantInventorySection source) :base(source)
	    {
		    Name = source.Name;
		    AllowsPartialBottles = source.AllowsPartialBottles;
		    IsDefaultInventorySection = source.IsDefaultInventorySection;
	    }

        public Restaurant.Restaurant Restaurant { get; set; }

        public string RestaurantId { get; set; }

        public string Name { get; set; }
        public bool AllowsPartialBottles { get; set; }

        public bool IsDefaultInventorySection { get; set; }

        protected override Core.Common.Entities.Inventory.RestaurantInventorySection CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.RestaurantInventorySection
            {
                Name = Name,
                AllowsPartialBottles = AllowsPartialBottles,
                IsDefaultInventorySection = IsDefaultInventorySection
            };
        }
    }
}
