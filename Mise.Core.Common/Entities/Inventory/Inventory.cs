using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
    public class Inventory : RestaurantEntityBase, IInventory
    {
        public Inventory()
        {
            Sections = new List<InventorySection>();
        }
        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new Inventory());
            newItem.Sections = Sections.Select(s => s.Clone() as InventorySection).ToList();
            newItem.DateCompleted = DateCompleted;
            newItem.CreatedByEmployeeID = CreatedByEmployeeID;
            newItem.IsCurrent = IsCurrent;
            return newItem;
        }

        public IEnumerable<IInventoryBeverageLineItem> GetBeverageLineItems()
        {
            return Sections.SelectMany(s => s.GetInventoryBeverageLineItemsInSection());
        }

        public List<InventorySection> Sections { get; set; }
        public IEnumerable<IInventorySection> GetSections()
        {
            return Sections;
        }

        public DateTimeOffset? DateCompleted { get; set; }

        public Guid CreatedByEmployeeID { get; set; }


        public bool IsCurrent { get; set; }

        public bool ContainsSearchString(string searchString)
        {
            return
                (DateCompleted != null && DateCompleted.ToString().ToUpper().Contains(searchString.ToUpper()))
                || (searchString.ToUpper() == "CURRENT" && IsCurrent)
                || (Sections != null && Sections.Any(s => s.ContainsSearchString(searchString)));
        }

        public void When(IInventoryEvent entityEvent)
        {
            LastUpdatedDate = entityEvent.CreatedDate;
            Revision = entityEvent.EventOrderingID;

            switch (entityEvent.EventType)
            {
                case MiseEventTypes.InventoryCreated:
                    WhenCreated((InventoryCreatedEvent)entityEvent);
                    break;
                case MiseEventTypes.InventoryCompleted:
                    WhenCompleted((InventoryCompletedEvent)entityEvent);
                    break;
                case MiseEventTypes.InventoryMadeCurrent:
                    WhenMadeCurrent((InventoryMadeCurrentEvent)entityEvent);
                    break;
                case MiseEventTypes.InventoryLiquidItemMeasured:
                    WhenInventoryItemMeasured((InventoryLiquidItemMeasuredEvent)entityEvent);
                    break;
                case MiseEventTypes.InventoryNewSectionAdded:
                    WhenInventoryNewSectionAdded((InventoryNewSectionAddedEvent)entityEvent);
                    break;
                case MiseEventTypes.InventorySectionCompleted:
                    WhenInventorySectionCompleted((InventorySectionCompletedEvent)entityEvent);
                    break;
                case MiseEventTypes.InventoryLineItemAdded:
                    WhenInventoryLineItemAdded((InventoryLineItemAddedEvent)entityEvent);
                    break;
                default:
                    throw new ArgumentException("Cannot use event " + entityEvent.EventType);
            }
        }

        private void WhenInventoryLineItemAdded(InventoryLineItemAddedEvent entityEvent)
        {
            var section = Sections.FirstOrDefault(s => s.ID == entityEvent.InventorySectionID);

            if (section == null)
            {
                throw new ArgumentException("Invalid section ID in inventory item add");
            }

            //calc the current amount
            var currentAmount = LiquidAmount.None;
            if (entityEvent.Container != null)
            {
                currentAmount = entityEvent.Container.AmountContained.Multiply(entityEvent.Quantity);
            }

            //make our new LI
            var newLI = new InventoryBeverageLineItem
            {
                NumFullBottles = entityEvent.Quantity,
                CurrentAmount = currentAmount,
                CaseSize = entityEvent.CaseSize,
                Container = entityEvent.Container,
                CreatedDate = entityEvent.CreatedDate,
                DisplayName = entityEvent.DisplayName,
                ID = entityEvent.LineItemID,
                LastUpdatedDate = entityEvent.CreatedDate,
                MethodsMeasuredLast = MeasurementMethods.Unmeasured,
                MiseName = entityEvent.MiseName,
                PricePaid = entityEvent.PricePaid,
                RestaurantID = entityEvent.RestaurantID,
                Revision = entityEvent.EventOrderingID,
                UPC = entityEvent.UPC,
                VendorBoughtFrom = entityEvent.VendorBoughtFrom,
                Categories = entityEvent.Categories.ToList(),
                InventoryPosition = entityEvent.InventoryPosition
            };

            section.LineItems.Add(newLI);

        }

        private void WhenMadeCurrent(InventoryMadeCurrentEvent entityEvent)
        {
            IsCurrent = true;
        }

        void WhenInventoryNewSectionAdded(InventoryNewSectionAddedEvent inventoryNewSectionAddedEvent)
        {
            var existing = GetSections().Any(s => s.ID == inventoryNewSectionAddedEvent.SectionID);
            if (existing == false)
            {
                var newSec = new InventorySection
                {
                    LineItems = new List<InventoryBeverageLineItem>(),
                    Name = inventoryNewSectionAddedEvent.Name,
                    RestaurantInventorySectionID = inventoryNewSectionAddedEvent.RestaurantSectionId,
                    RestaurantID = inventoryNewSectionAddedEvent.RestaurantID,
                    ID = inventoryNewSectionAddedEvent.SectionID,
                    Revision = inventoryNewSectionAddedEvent.EventOrderingID,
                    CreatedDate = inventoryNewSectionAddedEvent.CreatedDate,
                    LastUpdatedDate = inventoryNewSectionAddedEvent.CreatedDate,
                };
                Sections.Add(newSec);
            }
        }

        protected virtual void WhenInventorySectionCompleted(InventorySectionCompletedEvent inventorySectionCompletedEvent)
        {
            var section = GetSections().FirstOrDefault(s => s.ID == inventorySectionCompletedEvent.InventorySectionID);
            if (section == null)
            {
                throw new ArgumentException("Cannot find section for inventory section " + inventorySectionCompletedEvent.InventorySectionID);
            }

            section.LastCompletedBy = inventorySectionCompletedEvent.CausedByID;
        }

        protected virtual void WhenCompleted(InventoryCompletedEvent entityEvent)
        {
            //check all sections are completed!
            var undoneSections = GetSections().Where(s => s.Completed == false);
            foreach (var sec in undoneSections)
            {
                sec.LastCompletedBy = entityEvent.CausedByID;
            }
            DateCompleted = entityEvent.CreatedDate;
            IsCurrent = false;
            //TODO the service should make a copy, and mark that one as current
        }


        protected virtual void WhenCreated(InventoryCreatedEvent created)
        {
            CreatedDate = created.CreatedDate;
            ID = created.InventoryID;
            CreatedByEmployeeID = created.CausedByID;
            if (created.RestaurantID != Guid.Empty)
            {
                RestaurantID = created.RestaurantID;
            }
            DateCompleted = null;
            Sections = new List<InventorySection>();
            RestaurantID = created.RestaurantID;
        }

        protected virtual void WhenInventoryItemMeasured(InventoryLiquidItemMeasuredEvent entityEvent)
        {
            //find the section
            var section = Sections.FirstOrDefault(s => s.ID == entityEvent.InventorySectionID);
            if (section == null)
            {
                throw new ArgumentException("Section specified does not exist in inventory");
            }

            //determine how much of the item we should add
            var baseBeverageItem = entityEvent.BeverageLineItem;
            if (baseBeverageItem == null)
            {
                throw new ArgumentException("No BeverageItem given with event!");
            }

            if (LiquidAmount.None.GreaterThan(entityEvent.AmountMeasured))
            {
                throw new ArgumentException("Cannot measure a negative amount");
            }
            var lineItem = entityEvent.BeverageLineItem;
            lineItem.MethodsMeasuredLast = MeasurementMethods.VisualEstimate;

            lineItem.CurrentAmount = entityEvent.AmountMeasured;
            lineItem.NumFullBottles = entityEvent.NumFullBottlesMeasured;
            lineItem.PartialBottleListing = entityEvent.PartialBottles;


            var existingLI = section.LineItems.FirstOrDefault(li => li.ID == entityEvent.BeverageLineItem.ID);
            if (existingLI == null)
            {
                throw new ArgumentException("No inventory item of " + lineItem.DisplayName + " exists");
            }

            section.LineItems.Remove(existingLI);
            section.LineItems.Add(lineItem);

        }

    }
}
