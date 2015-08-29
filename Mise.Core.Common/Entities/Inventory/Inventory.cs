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
				case MiseEventTypes.InventorySectionCleared:
					WhenInventorySectionCleared ((InventorySectionClearedEvent)entityEvent);
					break;
                case MiseEventTypes.InventoryLineItemAdded:
                    WhenInventoryLineItemAdded((InventoryLineItemAddedEvent)entityEvent);
                    break;
				case MiseEventTypes.InventoryLineItemDeleted:
					WhenInventoryLineItemDeleted ((InventoryLineItemDeletedEvent)entityEvent);
					break;
				case MiseEventTypes.InventoryLineItemMovedToNewPosition:
					WhenInventoryLineItemMovedToNewPosition ((InventoryLineItemMovedToNewPositionEvent)entityEvent);
					break;
				case MiseEventTypes.InventorySectionStartedByEmployee:
					WhenInventorySectionStartedByEmployee ((InventorySectionStartedByEmployeeEvent)entityEvent);
					break;
                default:
                    throw new ArgumentException("Cannot use event " + entityEvent.EventType);
            }
        }

		void WhenInventoryLineItemMovedToNewPosition(InventoryLineItemMovedToNewPositionEvent ev){
			var section = Sections.FirstOrDefault (s => s.ID == ev.InventorySectionID);
			if(section == null){
				throw new ArgumentException("Section not found");
			}

			var item = section.LineItems.FirstOrDefault(li => li.ID == ev.LineItemID);
			if(item == null){
				throw new ArgumentException("Item not found");
			}

			MoveLineItemToPosition(section, item, ev.NewPositionWanted);
		}

		void MoveLineItemToPosition(InventorySection sec, InventoryBeverageLineItem li, int newPosition){
			//is there already an item at the destination?	If so move it one forward
			var existing = sec.LineItems.FirstOrDefault(l => l.InventoryPosition == newPosition);
			if(existing != null){
				MoveLineItemToPosition (sec, existing, newPosition + 1);
			}

			li.InventoryPosition = newPosition;
		}

		void WhenInventoryLineItemDeleted (InventoryLineItemDeletedEvent ev)
		{
			var section = Sections.FirstOrDefault (sec => sec.ID == ev.InventorySectionID);
			if(section == null)
			{
				throw new ArgumentException("Invalid section ID in inventory item add");
			}

			var lineItem = section.LineItems.FirstOrDefault (li => li.ID == ev.InventoryLineItemID);
			if (lineItem != null) {
				section.LineItems.Remove (lineItem);
			}
		}

        private void WhenInventoryLineItemAdded(InventoryLineItemAddedEvent entityEvent)
        {
			if(entityEvent.InventoryPosition < 10){
				throw new ArgumentException ("Attempting to add an item at position 0");
			}

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

			//double check we don't already have the inventory position
			var invPos = entityEvent.InventoryPosition;
			if(section.LineItems.Any())
			{
				var currentInvPositions = section.LineItems.Select (l => l.InventoryPosition).Distinct ().ToList();
				while(currentInvPositions.Contains (invPos)){
					invPos++;
				}
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
                RestaurantID = entityEvent.RestaurantID,
                Revision = entityEvent.EventOrderingID,
                UPC = entityEvent.UPC,
                VendorBoughtFrom = entityEvent.VendorBoughtFrom,
                Categories = entityEvent.Categories.ToList(),
				InventoryPosition = invPos
            };

            section.LineItems.Add(newLI);
			section.LineItems = section.LineItems.OrderBy (li => li.InventoryPosition).ToList ();

        }

		void WhenInventorySectionCleared (InventorySectionClearedEvent ev)
		{
			var section = Sections.FirstOrDefault (s => s.ID == ev.SectionId);
			if(section == null){
				throw new ArgumentException ("Invalid section");
			}

			section.LineItems.Clear ();
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
					InventoryID = inventoryNewSectionAddedEvent.InventoryID
                };
                Sections.Add(newSec);
            }
        }

        protected virtual void WhenInventorySectionCompleted(InventorySectionCompletedEvent inventorySectionCompletedEvent)
        {
            var section = Sections.FirstOrDefault(s => s.ID == inventorySectionCompletedEvent.InventorySectionID);
            if (section == null)
            {
                throw new ArgumentException("Cannot find section for inventory section " + inventorySectionCompletedEvent.InventorySectionID);
            }

            section.LastCompletedBy = inventorySectionCompletedEvent.CausedByID;
			section.CurrentlyInUseBy = null;
        }

		void WhenInventorySectionStartedByEmployee (InventorySectionStartedByEmployeeEvent ev)
		{
			var section = Sections.FirstOrDefault (s => s.ID == ev.InventorySectionId);
			if (section == null)
			{
				throw new ArgumentException("Cannot find section for inventory section " + ev.InventorySectionId);
			}

			section.CurrentlyInUseBy = ev.CausedByID;
			section.TimeCountStarted = ev.CreatedDate;
		}

        protected virtual void WhenCompleted(InventoryCompletedEvent entityEvent)
        {
            //check all sections are completed!  any not already assigned, complete them with this user
			foreach (var sec in Sections.Where(s => s.LastCompletedBy.HasValue == false))
            {
                sec.LastCompletedBy = entityEvent.CausedByID;
            }
            DateCompleted = entityEvent.CreatedDate;
            IsCurrent = false;
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
			var lineItem = entityEvent.BeverageLineItem.Clone() as InventoryBeverageLineItem;
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
