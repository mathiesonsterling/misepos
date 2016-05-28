﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.Entities.Vendor
{
    public class VendorRestaurantRelationships : EntityData
    {
        public VendorRestaurantRelationships() { }

        public VendorRestaurantRelationships(Vendor vendor, Restaurant.Restaurant restaurant)
        {
            Vendor = vendor;
            Restaurant = restaurant;
            Id = vendor.EntityId + ":" + restaurant.RestaurantID;
        }

        public Vendor Vendor { get; set; }
        public Restaurant.Restaurant Restaurant { get; set; }
    }
}
