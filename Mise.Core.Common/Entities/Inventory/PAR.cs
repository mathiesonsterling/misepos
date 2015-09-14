using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Common.Events.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
    public class Par : RestaurantEntityBase, IPar
    {

		public Par(){
			ParLineItems = new List<ParBeverageLineItem> ();
		}

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new Par());
            newItem.IsCurrent = IsCurrent;
            newItem.CreatedByEmployeeID = CreatedByEmployeeID;
            newItem.ParLineItems = ParLineItems;
            return newItem;
        }

        public Guid CreatedByEmployeeID { get; set; }
        public bool IsCurrent { get; set; }

        public List<ParBeverageLineItem> ParLineItems { get; set; }
        public IEnumerable<IParBeverageLineItem> GetBeverageLineItems()
        {
            return ParLineItems;
        }

        public void When(IParEvent entityEvent)
        {
			switch(entityEvent.EventType){
			case MiseEventTypes.PARCreated:
				WhenParCreated ((PARCreatedEvent)entityEvent);
				break;
			case MiseEventTypes.PARLineItemAdded:
				WhenLineItemAdded ((PARLineItemAddedEvent)entityEvent);
				break;
			case MiseEventTypes.PARLineItemQuantityUpdated:
				WhenLineItemQuantityUpdated ((PARLineItemQuantityUpdatedEvent)entityEvent);
				break;
			case MiseEventTypes.ParLineItemDeleted:
				WhenLineItemDeleted ((ParLineItemDeletedEvent)entityEvent);
				break;
			default:
				throw new InvalidOperationException ("Don't know how to handle event " + entityEvent.EventType);
			}

			LastUpdatedDate = entityEvent.CreatedDate;
			Revision = entityEvent.EventOrder;
        }

		void WhenParCreated (PARCreatedEvent pARCreatedEvent)
		{
			Id = pARCreatedEvent.ParID;
			CreatedDate = pARCreatedEvent.CreatedDate;
			CreatedByEmployeeID = pARCreatedEvent.CausedById;
			RestaurantID = pARCreatedEvent.RestaurantId;
			IsCurrent = true;
		}

		void WhenLineItemAdded (PARLineItemAddedEvent pARLineItemAddedEvent)
		{
			//make the li
			var li = new ParBeverageLineItem {
				Id = pARLineItemAddedEvent.LineItemID,
				CreatedDate = pARLineItemAddedEvent.CreatedDate,
				LastUpdatedDate = pARLineItemAddedEvent.CreatedDate,
				Revision = pARLineItemAddedEvent.EventOrder,
				RestaurantID = pARLineItemAddedEvent.RestaurantId,

				MiseName = pARLineItemAddedEvent.MiseName,
				UPC = pARLineItemAddedEvent.UPC,
				DisplayName = pARLineItemAddedEvent.DisplayName,
				Container = pARLineItemAddedEvent.Container,
				CaseSize = pARLineItemAddedEvent.CaseSize,
				Categories = pARLineItemAddedEvent.Categories.ToList()
			};

			if(pARLineItemAddedEvent.Quantity.HasValue){
				li.Quantity = pARLineItemAddedEvent.Quantity.Value;
			}
			ParLineItems.Add (li);
		}

		void WhenLineItemDeleted (ParLineItemDeletedEvent ev)
		{
			var lineItem = ParLineItems.FirstOrDefault (li => li.Id == ev.LineItemId);
			if(lineItem == null){
				throw new InvalidOperationException ("Can't find line item with ID " + ev.LineItemId);
			}

			ParLineItems.Remove (lineItem);
		}

		void WhenLineItemQuantityUpdated (PARLineItemQuantityUpdatedEvent pEvent)
		{
			var li = ParLineItems.FirstOrDefault (l => l.Id == pEvent.LineItemID);
			if(li == null){
				throw new InvalidOperationException ("Can't find line item with ID " + pEvent.LineItemID);
			}

			li.Quantity = pEvent.UpdatedQuantity;
		}
    }
}
