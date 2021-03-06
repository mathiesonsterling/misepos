﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Database.AzureDefinitions.Entities.Categories;
using Mise.Database.AzureDefinitions.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.Inventory
{
    public class PurchaseOrder : BaseDbEntity<IPurchaseOrder, Core.Common.Entities.Inventory.PurchaseOrder>
    {
        public PurchaseOrder() { }

        public PurchaseOrder(IPurchaseOrder source, IEnumerable<InventoryCategory> cats,
            IEnumerable<Vendor.Vendor> vendors, Restaurant.Restaurant restaurant, Employee createdByEmp)
            : base(source)
        {
            Restaurant = restaurant;
            RestaurantId = restaurant.Id;

            CreatedBy = createdByEmp;
            CreatedById = createdByEmp?.Id;

            PurchaseOrdersPerVendor =
                source.GetPurchaseOrderPerVendors().Select(pov => CreatePurchaseOrderForVendor(pov, cats, vendors)).ToList();
        }

        private static PurchaseOrderPerVendor CreatePurchaseOrderForVendor(IPurchaseOrderPerVendor pov, IEnumerable<InventoryCategory> cats,
            IEnumerable<Vendor.Vendor> allVendors)
        {
            var thisVendor = allVendors.FirstOrDefault(v => v.EntityId == pov.VendorID);
            return new PurchaseOrderPerVendor(pov, thisVendor, cats);
        }
           
        [Required]
        public Restaurant.Restaurant Restaurant
        {
            get;
            set;
        }
        [ForeignKey("Restaurant")]
        public string RestaurantId { get; set; }

        public List<PurchaseOrderPerVendor> PurchaseOrdersPerVendor { get; set; }

        public Employee CreatedBy{ get; set; }
        [ForeignKey("CreatedBy")]
        public string CreatedById { get; set; }

        protected override Core.Common.Entities.Inventory.PurchaseOrder CreateConcreteSubclass()
        {
            return new Core.Common.Entities.Inventory.PurchaseOrder
            {
                PurchaseOrdersPerVendor = PurchaseOrdersPerVendor.Select(pv => pv.ToBusinessEntity()).ToList(),
                CreatedByEmployeeID = CreatedBy?.EntityId,
                CreatedBy = CreatedBy?.GetName()
            };
        }
    }
}
