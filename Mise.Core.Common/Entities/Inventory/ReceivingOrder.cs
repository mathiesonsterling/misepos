﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.ValueItems;


namespace Mise.Core.Common.Entities.Inventory
{
	public class ReceivingOrder : RestaurantEntityBase, IReceivingOrder
	{
		public DateTimeOffset DateReceived{ get; set;}

        /// <summary>
        /// If set, the purchase order this RO is associated with
        /// </summary>
        public Guid? PurchaseOrderID { get; set; }

	    public DateTimeOffset? PurchaseOrderDate { get; set; }

	    public Guid VendorID { get; set; }
        public ReceivingOrderStatus Status { get; set; }
        public Guid ReceivedByEmployeeID { get; set; }
	    public string Notes { get; set; }

		public string InvoiceID {
			get;
			set;
		}

	    public ReceivingOrder()
	    {
            LineItems = new List<ReceivingOrderLineItem>();
	    }

        public List<ReceivingOrderLineItem> LineItems { get; set; } 
	    public IEnumerable<IReceivingOrderLineItem> GetBeverageLineItems()
	    {
	        return LineItems;
	    } 

        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new ReceivingOrder());
            newItem.ReceivedByEmployeeID = ReceivedByEmployeeID;
            newItem.PurchaseOrderID = PurchaseOrderID;
            newItem.LineItems = LineItems.Select(li => li.Clone() as ReceivingOrderLineItem).ToList();
            newItem.Status = Status;
            return newItem;
        }


		public bool ContainsSearchString (string searchString)
		{
			return Notes.Contains (searchString) 
				|| Status.ToString ().Contains (searchString)
				|| LineItems.Any (li => li.ContainsSearchString (searchString));
		}

	    public void When(IReceivingOrderEvent entityEvent)
	    {
	        switch (entityEvent.EventType)
	        {
	            case MiseEventTypes.ReceivingOrderCreated:
	                WhenReceivingOrderCreated((ReceivingOrderCreatedEvent) entityEvent);
	                break;
				case MiseEventTypes.ReceivingOrderCompleted:
					WhenReceivingOrderCompleted ((ReceivingOrderCompletedEvent)entityEvent);
					break;
				case MiseEventTypes.ReceivingOrderNoteAdded:
					WhenNoteAddedToReceivingOrder ((ReceivingOrderNoteAddedEvent)entityEvent);
				break;
				case MiseEventTypes.ReceivingOrderLineItemAdded:
					WhenReceivingOrderLineItemAdded ((ReceivingOrderLineItemAddedEvent)entityEvent);
					break;
				case MiseEventTypes.ReceivingOrderLineItemQuantityUpdated:
					WhenLIQuantityUpdated ((ReceivingOrderLineItemQuantityUpdatedEvent)entityEvent);
					break;
				case MiseEventTypes.ReceivingOrderLineItemZeroedOut:
					WhenLIZeroedOut ((ReceivingOrderLineItemZeroedOutEvent)entityEvent);
					break;
				case MiseEventTypes.ReceivingOrderLineItemDeleted:
					WhenLineItemDeleted ((ReceivingOrderLineItemDeletedEvent)entityEvent);
					break;
				case MiseEventTypes.ReceivingOrderAssociatedWithPO:
					WhenReceivingOrderAssociatedWithPurchaseOrder ((ReceivingOrderAssociatedWithPOEvent)entityEvent);
					break;
                default:
                    throw new ArgumentException("Don't know how to handle event " + entityEvent.EventType);
	        }

			LastUpdatedDate = entityEvent.CreatedDate;
			Revision = entityEvent.EventOrder;
	    }

	    protected virtual void WhenReceivingOrderCreated(ReceivingOrderCreatedEvent entityEvent)
	    {
	        Id = entityEvent.ReceivingOrderID;
	        ReceivedByEmployeeID = entityEvent.CausedById;
	        CreatedDate = entityEvent.CreatedDate;
	        Status = ReceivingOrderStatus.Created;
			VendorID = entityEvent.VendorID;
			RestaurantID = entityEvent.RestaurantId;
	    }

		void WhenReceivingOrderAssociatedWithPurchaseOrder (ReceivingOrderAssociatedWithPOEvent ev)
		{
			PurchaseOrderID = ev.PurchaseOrderID;
			PurchaseOrderDate = ev.PurchaseOrderSentDate;
		}

		void WhenReceivingOrderLineItemAdded (ReceivingOrderLineItemAddedEvent roEvent)
		{
			var newLI = new ReceivingOrderLineItem {
				Id = roEvent.LineItemID,
				CreatedDate = roEvent.CreatedDate,
				LastUpdatedDate = roEvent.CreatedDate,
				Revision = roEvent.EventOrder,
				RestaurantID = roEvent.RestaurantId,
					
				DisplayName = roEvent.DisplayName,
				CaseSize = roEvent.CaseSize,
				Container = roEvent.Container,
				MiseName = roEvent.MiseName,
				Quantity = roEvent.Quantity,
				UPC = roEvent.UPC,
				Categories = roEvent.Categories.ToList (),
				ZeroedOut = false
			};
					
			LineItems.Add (newLI);
		}

		void WhenLIQuantityUpdated (ReceivingOrderLineItemQuantityUpdatedEvent quEvent)
		{
			var perUnitPrice = quEvent.UpdatedQuantity > 0 
				? new Money(quEvent.LineItemPrice.Dollars/quEvent.UpdatedQuantity) 
				: Money.None;
			var li = LineItems.FirstOrDefault (l => l.Id == quEvent.LineItemID);
			if(li != null){
				li.Quantity = quEvent.UpdatedQuantity;
				li.Revision = quEvent.EventOrder;
				li.LastUpdatedDate = quEvent.CreatedDate;
				li.LineItemPrice= quEvent.LineItemPrice;
				li.UnitPrice = perUnitPrice;
				li.ZeroedOut = false;
			}
		}

		void WhenLineItemDeleted (ReceivingOrderLineItemDeletedEvent ev)
		{
			var lineItem = LineItems.FirstOrDefault (l => l.Id == ev.LineItemId);
			if(lineItem == null){
				throw new ArgumentException ("No line item found for ID " + ev.LineItemId);
			}

			LineItems.Remove (lineItem);
		}

		void WhenLIZeroedOut (ReceivingOrderLineItemZeroedOutEvent ev)
		{
			var li = LineItems.FirstOrDefault (l => l.Id == ev.LineItemID);
			li.Quantity = 0;
			li.LineItemPrice = Money.None;
			li.UnitPrice = Money.None;
			li.Revision = ev.EventOrder;
			li.LastUpdatedDate = ev.CreatedDate;
			li.ZeroedOut = true;
		}

		protected virtual void WhenReceivingOrderCompleted(ReceivingOrderCompletedEvent entityEvent){
			Status = ReceivingOrderStatus.Completed;
			Notes = entityEvent.Notes;
			InvoiceID = entityEvent.InvoiceID;
			DateReceived = entityEvent.DateReceived;
		}

		protected virtual void WhenNoteAddedToReceivingOrder(ReceivingOrderNoteAddedEvent noteEvent){
			Notes = noteEvent.Note;
		}
	}
}

