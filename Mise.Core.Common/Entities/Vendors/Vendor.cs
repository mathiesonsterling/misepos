using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Vendors;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Inventory;
using Mise.Core.Common.Entities.Inventory;
namespace Mise.Core.Common.Entities.Vendors
{
	public class Vendor : EntityBase, IVendor
	{
       public StreetAddress StreetAddress{get;set;}

	    public EmailAddress EmailToOrderFrom { get; set; }

	    public Uri Website { get; set; }

	    public PhoneNumber PhoneNumber{get;set;}

		public string Detail {
			get {
				var res = StreetAddress != null && StreetAddress.City != null 
					? StreetAddress.City.Name : String.Empty;

				if(StreetAddress != null && StreetAddress.State != null){
					if(res.Length > 0){
						res = res + ", ";
					}
					res = res + StreetAddress.State.Name;
				}

				return res;
			}
		}

        public Guid? CreatedByEmployeeID { get; set; }

	    public bool Verified { get; set; }

	    public IEnumerable<IVendorBeverageLineItem> GetItemsVendorSells()
	    {
	        return VendorBeverageLineItems;
	    }

	    public IEnumerable<Guid> GetRestaurantIDsAssociatedWithVendor()
	    {
	        return RestaurantsAssociatedIDs;
	    }

	    public List<Guid> RestaurantsAssociatedIDs { get; set; }
        public List<VendorBeverageLineItem> VendorBeverageLineItems { get; set; }
	    public Vendor()
	    {
	        RestaurantsAssociatedIDs = new List<Guid>();
            VendorBeverageLineItems = new List<VendorBeverageLineItem>();
	    }

		#region IVendor implementation


		public string Name {
			get;
			set;
		}

		#endregion

        public ICloneableEntity Clone()
        {
            var newItem = CloneEntityBase(new Vendor());
            newItem.Name = Name;
            newItem.StreetAddress = StreetAddress;
            newItem.PhoneNumber = PhoneNumber;
            newItem.VendorBeverageLineItems = VendorBeverageLineItems.Select(bi => bi.Clone()).Cast<VendorBeverageLineItem>().ToList();
            newItem.RestaurantsAssociatedIDs = RestaurantsAssociatedIDs;
            newItem.EmailToOrderFrom = EmailToOrderFrom;
            return newItem;
        }

		public bool ContainsSearchString (string searchString)
		{
			if( (Name != null && Name.ToUpper ().Contains (searchString.ToUpper ()))
			|| (StreetAddress != null && StreetAddress.ContainsSearchString (searchString))
            || (EmailToOrderFrom != null && EmailToOrderFrom.ContainsSearchString(searchString))){
		        return true;
		    }

			return  VendorBeverageLineItems.Any (li => li.ContainsSearchString (searchString));

		}

		public Money GetPriceForLineItem (IBaseBeverageLineItem li, decimal quantity, Guid? restaurantID)
		{
			var item = GetItemsVendorSells ()
				.FirstOrDefault(vLi => BeverageLineItemEquator.AreSameBeverageLineItem (vLi, li));

			//for basic restaurants, just return the price
			if(item != null){
			    if (restaurantID.HasValue)
			    {
			        return item.GetLastPricePaidByRestaurantPerUnit(restaurantID.Value) ?? item.PublicPricePerUnit;
			    }
			    return item.PublicPricePerUnit;
			}
			return null;
		}

		public bool DoesCarryItem (IBaseBeverageLineItem li, decimal quantity)
		{
			return GetItemsVendorSells ().Any (vLI => BeverageLineItemEquator.AreSameBeverageLineItem (vLI, li));
		}

	    public bool IsSameVendor(IVendor v)
	    {
	        if (v == null)
	        {
	            return false;
	        }

	        if (string.IsNullOrEmpty(v.Name))
	        {
	            return false;
	        }

            if(v.Name != Name)
	        {
	            return false;
	        }

			if (v.Name == Name && v.StreetAddress != null 
				&& v.StreetAddress.City.Equals(StreetAddress.City)
				&& v.StreetAddress.State.Equals(StreetAddress.State)
			) {
				return true;
			}

	        if (v.StreetAddress == null && StreetAddress != null)
	        {
	            return false;
	        }
	        if (StreetAddress == null && v.StreetAddress != null)
	        {
	            return false;
	        }
	        if (StreetAddress != null && v.StreetAddress != null && (v.StreetAddress.Equals(StreetAddress) == false))
	        {
	            return false;
	        }

	        if (EmailToOrderFrom != null && v.EmailToOrderFrom != null)
	        {
	            return EmailToOrderFrom.Equals(EmailToOrderFrom);
	        }

	        return true;
	    }

	    #region Event sourcing
        public void When(IVendorEvent entityEvent)
        {
            LastUpdatedDate = entityEvent.CreatedDate;
            Revision = entityEvent.EventOrder;
	        switch (entityEvent.EventType)
	        {
	            case MiseEventTypes.VendorCreatedEvent:
	                WhenVendorCreated((VendorCreatedEvent)entityEvent);
	                break;
                case MiseEventTypes.VendorAddressUpdated:
	                WhenVendorAddressUpdated((VendorAddressUpdatedEvent) entityEvent);
	                break;
                case MiseEventTypes.VendorPhoneNumberUpdated:
	                WhenVendorPhoneNumberUpdated((VendorPhoneNumberUpdatedEvent) entityEvent);
	                break;
                case MiseEventTypes.RestaurantAssociatedWithVendor:
	                WhenRestaurantAssociatedWithVendor((RestaurantAssociatedWithVendorEvent) entityEvent);
	                break;
				case MiseEventTypes.VendorLineItemAdded:
					WhenLineItemAdded ((VendorAddNewLineItemEvent)entityEvent);
					break;
			case MiseEventTypes.VendorRestaurantSetsPriceForReceivedItem:
				WhenLineItemPriceSet ((VendorRestaurantSetsPriceForReceivedItemEvent)entityEvent);
				break;
                default:
                    throw new ArgumentException("Don't know how to handle " + entityEvent.EventType);
	        }
	    }

	    protected virtual void WhenRestaurantAssociatedWithVendor(RestaurantAssociatedWithVendorEvent entityEvent)
	    {
	        if (RestaurantsAssociatedIDs.Contains(entityEvent.RestaurantId) == false)
	        {
	            RestaurantsAssociatedIDs.Add(entityEvent.RestaurantId);
	        }
	    }

	    protected virtual void WhenVendorPhoneNumberUpdated(VendorPhoneNumberUpdatedEvent entityEvent)
	    {
	        PhoneNumber = entityEvent.PhoneNumber;
	    }

	    protected virtual void WhenVendorAddressUpdated(VendorAddressUpdatedEvent entityEvent)
	    {
	        StreetAddress = entityEvent.StreetAddress;
	    }

	    protected virtual void WhenVendorCreated(VendorCreatedEvent created)
	    {
            CreatedDate = created.CreatedDate;
	        Id = created.VendorID;
	        CreatedByEmployeeID = created.CausedById;

			Name = created.Name;
			StreetAddress = created.Address;
			PhoneNumber = created.PhoneNumber;
	        EmailToOrderFrom = created.Email;

			if(RestaurantsAssociatedIDs.Contains (created.RestaurantId)==false){
				RestaurantsAssociatedIDs.Add (created.RestaurantId);
			}
	    }


		void WhenLineItemAdded (VendorAddNewLineItemEvent nLIEvent)
		{
			var newLI = new VendorBeverageLineItem {
				Id = nLIEvent.LineItemID,
				CaseSize = nLIEvent.CaseSize,
				Container = nLIEvent.Container,
				CreatedDate = nLIEvent.CreatedDate,
				DisplayName = nLIEvent.DisplayName,
				LastTimePriceSet = nLIEvent.CreatedDate,
				LastUpdatedDate = nLIEvent.CreatedDate,
				MiseName = nLIEvent.MiseName,
				Revision = nLIEvent.EventOrder,
				UPC = nLIEvent.UPC,
				VendorID = Id
			};
			VendorBeverageLineItems.Add (newLI);
		}

		void WhenLineItemPriceSet (VendorRestaurantSetsPriceForReceivedItemEvent ev)
		{
			var lineItem = VendorBeverageLineItems.FirstOrDefault (li => li.Id == ev.VendorLineItemID);
			if (lineItem == null) {
				throw new InvalidOperationException ("No line item exists for ID " + ev.VendorLineItemID);
			}

		    lineItem.PricePerUnitForRestaurant[ev.RestaurantId] = ev.PricePerUnit;
		    lineItem.LastTimePriceSet = DateTime.UtcNow;

			if (RestaurantsAssociatedIDs.Contains(ev.RestaurantId) == false)
			{
				RestaurantsAssociatedIDs.Add(ev.RestaurantId);
			}
		}

        #endregion
    }
}

