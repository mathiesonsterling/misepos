using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;

namespace Mise.Core.Common.Entities
{
	public class Restaurant : RestaurantEntityBase, IRestaurant
    {
        public Restaurant()
        {
            DiscountAmounts = new List<DiscountAmount>();
            DiscountPercentages = new List<DiscountPercentage>();
            DiscountPercentageAfterMinimumCashTotals = new List<DiscountPercentageAfterMinimumCashTotal>();
            Terminals = new List<MiseTerminalDevice>();
            InventorySections = new List<RestaurantInventorySection>();
        }

        public ICloneableEntity Clone()
        {
            var newItem = base.CloneRestaurantBase(new Restaurant());
            newItem.AccountID = AccountID;
            newItem.FriendlyID = FriendlyID;
            newItem.InventorySections = InventorySections.Select(i => i.Clone() as RestaurantInventorySection).ToList();
            newItem.Name = Name;
            newItem.NumberOfActiveCashRegisters = NumberOfActiveCashRegisters;
            newItem.NumberOfActiveCreditRegisters = NumberOfActiveCreditRegisters;
            newItem.NumberOfActiveOrderTerminals = NumberOfActiveOrderTerminals;
            newItem.PhoneNumber = PhoneNumber;
            newItem.RestaurantServerLocation = RestaurantServerLocation;
            newItem.StreetAddress = StreetAddress;
            newItem.Terminals = Terminals;
            newItem.LastMeasuredInventoryID = LastMeasuredInventoryID;
            newItem.CurrentInventoryID = CurrentInventoryID;
           
            return newItem;
        }

		public Guid? AccountID {
			get;
			set;
		}

        public string FriendlyID { get; set; }

        public List<MiseTerminalDevice> Terminals { get; set; }

        public IEnumerable<IMiseTerminalDevice> GetTerminals()
        {
            return Terminals;
        }

        public void AddTerminal(IMiseTerminalDevice device)
        {
            var concrete = device as MiseTerminalDevice;
            if (concrete != null)
            {
                Terminals.Add(concrete);
            }
        }

        public Uri RestaurantServerLocation { get; set; }


        public List<RestaurantInventorySection> InventorySections { get; set; }


        public Guid? LastMeasuredInventoryID { get; set; }

        public IEnumerable<IRestaurantInventorySection> GetInventorySections()
        {
            return InventorySections;
        }

        public RestaurantName Name { get; set; }

		public StreetAddress StreetAddress {
			get;
			set;
		}
		public PhoneNumber PhoneNumber {
			get;
			set;
		}

        public List<DiscountAmount> DiscountAmounts;
        public List<DiscountPercentage> DiscountPercentages;
        public List<DiscountPercentageAfterMinimumCashTotal> DiscountPercentageAfterMinimumCashTotals;

		public IEnumerable<IDiscount> GetPossibleDiscounts(){
            var list = new List<IDiscount>();
            list.AddRange(DiscountAmounts);
            list.AddRange(DiscountPercentages);
            list.AddRange(DiscountPercentageAfterMinimumCashTotals);

            return list;
		}
 
        public int NumberOfActiveCashRegisters { get; set; }
        public int NumberOfActiveCreditRegisters { get; set; }
        public int NumberOfActiveOrderTerminals { get; set; }

        /// <summary>
        /// If this is true, this is just here to support a user, but is not verified as a real restaurant
        /// </summary>
        public bool IsPlaceholder { get { return AccountID.HasValue == false; } }

        public Guid? CurrentInventoryID { get; set; }

        public void When(IRestaurantEvent entityEvent)
        {
            switch (entityEvent.EventType)
            {
                case MiseEventTypes.PlaceholderRestaurantCreated:
                    WhenPlaceholderRestaurantCreated((PlaceholderRestaurantCreatedEvent) entityEvent);
                    break;
                case MiseEventTypes.InventorySectionAddedToRestaurant:
                    WhenInventorySectionAdded((InventorySectionAddedToRestaurantEvent) entityEvent);
                    break;
                case MiseEventTypes.NewRestaurantRegisteredOnApp:
                    WhenNewRestaurantCreatedOnApp((NewRestaurantRegisteredOnAppEvent) entityEvent);
                    break;
				case MiseEventTypes.UserSelectedRestaurant:
					WhenUserSelectedRestaurant ((UserSelectedRestaurant)entityEvent);
					break;
                case MiseEventTypes.RestaurantAssignedToAccount:
                    WhenRestaurantAssignedToAccount((RestaurantAssignedToAccountEvent)entityEvent);
                    break;
                default:
                    throw new ArgumentException("Can't handle event " + entityEvent.EventType);
            }

            LastUpdatedDate = entityEvent.CreatedDate;
            Revision = entityEvent.EventOrder;
        }

	    private void WhenNewRestaurantCreatedOnApp(NewRestaurantRegisteredOnAppEvent ev)
	    {
	        Name = ev.Name;
	        Id = ev.RestaurantId;
			RestaurantID = ev.RestaurantId;
	        CreatedDate = ev.CreatedDate;
	        StreetAddress = ev.StreetAddress;
	        PhoneNumber = ev.PhoneNumber;
	    }

	    private void WhenPlaceholderRestaurantCreated(PlaceholderRestaurantCreatedEvent entityEvent)
        {
            AccountID = null;
            Id = entityEvent.RestaurantId;
            RestaurantID = entityEvent.RestaurantId;
            CreatedDate = entityEvent.CreatedDate;
        }

        private void WhenInventorySectionAdded(InventorySectionAddedToRestaurantEvent entityEvent)
        {
            var section = InventorySections.FirstOrDefault(s => s.Id == entityEvent.SectionID);
            if (section != null) return;

            section = new RestaurantInventorySection
            {
                Id = entityEvent.SectionID,
                Name = entityEvent.SectionName,
                CreatedDate = entityEvent.CreatedDate,
                LastUpdatedDate = entityEvent.CreatedDate,
                Revision = entityEvent.EventOrder,
                RestaurantID = entityEvent.RestaurantId,
				AllowsPartialBottles = entityEvent.AllowsPartialBottles
            };
            InventorySections.Add(section);
        }

		void WhenUserSelectedRestaurant (UserSelectedRestaurant userSelectedRestaurant)
		{
			//do nothing for now, mostly used for caching
		}

        void WhenRestaurantAssignedToAccount(RestaurantAssignedToAccountEvent ev){
            AccountID = ev.AccountId;
        }
    }
}
