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
					WhenInventoryNewSectionAdded ((InventoryNewSectionAddedEvent)entityEvent);
					break;
				case MiseEventTypes.InventorySectionCompleted:
					WhenInventorySectionCompleted ((InventorySectionCompletedEvent)entityEvent);
					break;
                case MiseEventTypes.InventoryLineItemAdded:
                    WhenInventoryLineItemAdded((InventoryLineItemAddedEvent) entityEvent);
                    break;
                default:
                    throw new ArgumentException("Cannot use event " + entityEvent.EventType);
            }
        }

        private void WhenInventoryLineItemAdded(InventoryLineItemAddedEvent entityEvent)
        {
            InventorySection section;
            if (entityEvent.RestaurantInventorySectionID.HasValue)
            {
                section = Sections.FirstOrDefault(s => s.RestaurantInventorySectionID == entityEvent.RestaurantInventorySectionID);
                if (section == null)
                {
                    throw new ArgumentException("No section found for ID " + entityEvent.RestaurantInventorySectionID);
                }
            }
            else
            {
               //get the most populated one!
                var mostPopulatedSection =
                    Sections.OrderByDescending(s => s.GetInventoryBeverageLineItemsInSection().Count()).First();
                section = mostPopulatedSection;
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
				Categories = entityEvent.Categories.ToList (),
                InventoryPosition = entityEvent.InventoryPosition
            };

            section.LineItems.Add(newLI);

        }

        private void WhenMadeCurrent(InventoryMadeCurrentEvent entityEvent)
        {
            IsCurrent = true;
        }

		void WhenInventoryNewSectionAdded (InventoryNewSectionAddedEvent inventoryNewSectionAddedEvent)
		{
			var existing = GetSections ().Any (s => s.RestaurantInventorySectionID == inventoryNewSectionAddedEvent.RestaurantSectionId);
			if (existing == false) {
				var newSec = new InventorySection {
					LineItems = new List<InventoryBeverageLineItem>(),
					Name = inventoryNewSectionAddedEvent.Name,
					RestaurantInventorySectionID = inventoryNewSectionAddedEvent.RestaurantSectionId,
					RestaurantID = inventoryNewSectionAddedEvent.RestaurantID,
					ID = inventoryNewSectionAddedEvent.SectionID,
					Revision = inventoryNewSectionAddedEvent.EventOrderingID,
					CreatedDate = inventoryNewSectionAddedEvent.CreatedDate,
					LastUpdatedDate = inventoryNewSectionAddedEvent.CreatedDate,
				};
				Sections.Add (newSec);
			}
		}

		protected virtual void WhenInventorySectionCompleted (InventorySectionCompletedEvent inventorySectionCompletedEvent)
		{
			var section = GetSections ().FirstOrDefault(s => s.RestaurantInventorySectionID == inventorySectionCompletedEvent.RestaurantSectionID);
			if(section == null){
				throw new ArgumentException ("Cannot find section for inventory section " + inventorySectionCompletedEvent.RestaurantSectionID);
			}

			section.Completed = true;
		}

        protected virtual void WhenCompleted(InventoryCompletedEvent entityEvent)
        {
			//check all sections are completed!
			var undoneSections = GetSections ().Where(s => s.Completed == false);
			foreach(var sec in undoneSections){
				sec.Completed = true;
			}
            DateCompleted = entityEvent.CreatedDate;
            IsCurrent = false;
            //TODO the service should make a copy, and mark that one as current
        }
			

        protected virtual void WhenCreated(InventoryCreatedEvent created)
        {
            var inventorySections = created.RestaurantSectionsAndSectionIDs
                .Select(ri => CreateSectionFromRestaurantSection(ri.Item1.ID, ri.Item1.Name, created, ri.Item2));

            CreatedDate = created.CreatedDate;
            ID = created.InventoryID;
            CreatedByEmployeeID = created.CausedByID;
            if (created.RestaurantID != Guid.Empty)
            {
                RestaurantID = created.RestaurantID;
            }
            DateCompleted = null;
            Sections = inventorySections.ToList();
			RestaurantID = created.RestaurantID;
        }

        protected virtual void WhenInventoryItemMeasured(InventoryLiquidItemMeasuredEvent entityEvent)
        {
            //find the section
            var section = Sections.FirstOrDefault(s => s.RestaurantInventorySectionID == entityEvent.RestaurantInventorySectionID);
            if (section == null)
            {
                throw new ArgumentException("No section for RestaurantInventoryID " + entityEvent.RestaurantInventorySectionID);
            }

            //determine how much of the item we should add
            var baseBeverageItem = entityEvent.BeverageLineItem;
            if (baseBeverageItem == null)
            {
                throw new ArgumentException("No BeverageItem given with event!");
            }

            var lineItem = entityEvent.BeverageLineItem;
            lineItem.MethodsMeasuredLast = MeasurementMethods.VisualEstimate;
            lineItem.CurrentAmount = entityEvent.AmountMeasured;
			lineItem.NumFullBottles = entityEvent.NumFullBottlesMeasured;
			lineItem.PartialBottleListing = entityEvent.PartialBottles;

            //TODO - if the inventory already has this line item, we should add to it, or overwrite it?
			var existingLI = section.LineItems.FirstOrDefault (li => li.ID == entityEvent.BeverageLineItem.ID);
			if(existingLI != null){
				section.LineItems.Remove (existingLI);
			}
            section.LineItems.Add(lineItem);

        }

        private InventorySection CreateSectionFromRestaurantSection(Guid restSectionID, string sectionName, BaseInventoryEvent restEvent, Guid sectionID)
        {
            return new InventorySection
            {
                ID = sectionID,
                Completed = false,
                LineItems = new List<InventoryBeverageLineItem>(),
                Name = sectionName,
                RestaurantInventorySectionID = restSectionID,
                RestaurantID = restEvent.RestaurantID,
                CreatedDate = restEvent.CreatedDate,
                LastUpdatedDate = restEvent.CreatedDate,
                Revision = restEvent.EventOrderingID
            };
        }
    }
}
