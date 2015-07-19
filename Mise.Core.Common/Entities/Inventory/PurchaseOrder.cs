using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	public class PurchaseOrder : RestaurantEntityBase, IPurchaseOrder
	{
	    public PurchaseOrder()
	    {
            PurchaseOrdersPerVendor = new List<PurchaseOrderPerVendor>();
	    }

		public List<PurchaseOrderPerVendor> PurchaseOrdersPerVendor{ get; set; }


	    public IEnumerable<IPurchaseOrderLineItem> GetPurchaseOrderLineItems()
	    {
			return PurchaseOrdersPerVendor.SelectMany (piv => piv.LineItems);
	    }

		public IEnumerable<IPurchaseOrderPerVendor> GetPurchaseOrderPerVendors ()
		{
			return PurchaseOrdersPerVendor;
		}
			
		public string DisplayName {
			get {
				return CreatedDate.ToString ("dd/MM/yy");
			}
		}

		public string CreatedByName{ get; set;}
		public string DetailDisplay {
			get {
				return "Sent by " + CreatedByName;
			}
		}

	    public Guid CreatedByEmployeeID { get; set; }
        public ICloneableEntity Clone()
        {
            var newItem = CloneRestaurantBase(new PurchaseOrder());
            newItem.CreatedByEmployeeID = CreatedByEmployeeID;
			newItem.PurchaseOrdersPerVendor = PurchaseOrdersPerVendor
				.Select (pv => pv.Clone () as PurchaseOrderPerVendor).ToList();
            return newItem;
        }

		public void When (IPurchaseOrderEvent entityEvent)
		{
		    LastUpdatedDate = entityEvent.CreatedDate;
		    Revision = entityEvent.EventOrderingID;

		    switch (entityEvent.EventType)
		    {
		        case MiseEventTypes.PurchaseOrderCreated:
		            WhenPurchaseOrderCreated((PurchaseOrderCreatedEvent) entityEvent);
		            break;
			case MiseEventTypes.PurchaseOrderLineItemAddedFromInventoryCalculation:
				WhenPurchaseOrderLineItemAddedFromInventoryCalculation (
					(PurchaseOrderLineItemAddedFromInventoryCalculationEvent)entityEvent);
		            break;
                case MiseEventTypes.PurchaseOrderSentToVendor:
		            WhenPurchaseOrderSentToVendor((PurchaseOrderSentToVendorEvent) entityEvent);
		            break;
			case MiseEventTypes.PurchaseOrderReceivedFromVendor:
				WhenPurchaseOrderReceivedFromVendor ((PurchaseOrderRecievedFromVendorEvent)entityEvent);
				break;
                default:
                    throw new ArgumentException("Don't know how to handle event " + entityEvent.EventType);
		    }
		}

        void WhenPurchaseOrderCreated(PurchaseOrderCreatedEvent entityEvent)
        {
            ID = entityEvent.PurchaseOrderID;
            RestaurantID = entityEvent.RestaurantID;
            CreatedByEmployeeID = entityEvent.CausedByID;
            CreatedDate = entityEvent.CreatedDate;

			CreatedByName = entityEvent.EmployeeCreatingName;
        }

        void WhenPurchaseOrderLineItemAddedFromInventoryCalculation (PurchaseOrderLineItemAddedFromInventoryCalculationEvent ev)
		{
			//create our new item
			var amtBottles = 0;
			if (ev.AmountNeeded != null) {
                //TODO calculate how many bottles this is
			} else {
			    if (ev.NumBottlesNeeded.HasValue)
			    {
			        amtBottles = ev.NumBottlesNeeded.Value;
			    }
			    else
			    {
			        throw new InvalidOperationException("No value given for the amount needed");
			    }
			}

			//do we have an entry for the vendor already?
			var existing = PurchaseOrdersPerVendor.FirstOrDefault (pv => pv.VendorID == ev.VendorWithBestPriceID);
			if(existing == null){
				existing = new PurchaseOrderPerVendor {
					VendorID = ev.VendorWithBestPriceID,
					Status = PurchaseOrderStatus.Created
				};
				PurchaseOrdersPerVendor.Add (existing);
			}

			//todo - if we have already an item with same MiseName, DispalyName, container, VendorID, then add?
			var newLI = new PurchaseOrderLineItem {
				ID = ev.LineItemID,
				Revision = ev.EventOrderingID,
				CreatedDate = ev.CreatedDate,
				LastUpdatedDate = ev.CreatedDate,

				MiseName = ev.PARLineItem.MiseName,
				DisplayName = ev.PARLineItem.DisplayName,
				Container = ev.PARLineItem.Container,

				Quantity = amtBottles,
				VendorID = ev.VendorWithBestPriceID,
				Categories = ev.Categories.ToList ()
			};
				
			existing.LineItems.Add (newLI);
		}

	    protected virtual void WhenPurchaseOrderSentToVendor(PurchaseOrderSentToVendorEvent entityEvent)
	    {
			var items = PurchaseOrdersPerVendor.Where (pv => pv.VendorID == entityEvent.VendorID);
			foreach(var pv in items){
				pv.Status = PurchaseOrderStatus.SentToVendor;
			}
	    }
			
		void WhenPurchaseOrderReceivedFromVendor (PurchaseOrderRecievedFromVendorEvent ev)
		{
			var items = PurchaseOrdersPerVendor.Where (pv => pv.VendorID == ev.VendorID);
			foreach(var pv in items){
				pv.Status = ev.Status;
			}
		}

		public bool ContainsSearchString (string searchString)
		{
			return PurchaseOrdersPerVendor.Any (pov => pov.ContainsSearchString (searchString));
		}
	}
}

