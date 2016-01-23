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
using Mise.Core.Common.Entities.Accounts;
namespace Mise.Core.Common.Entities
{
	public class Restaurant : RestaurantEntityBase, IRestaurant
    {
        public Restaurant()
        {
            InventorySections = new List<RestaurantInventorySection>();
            EmailsToSendInventoryReportsTo = new List<EmailAddress>();
        }

        public ICloneableEntity Clone()
        {
            var newItem = base.CloneRestaurantBase(new Restaurant());
            newItem.AccountID = AccountID;
            newItem.InventorySections = InventorySections.Select(i => i.Clone() as RestaurantInventorySection).ToList();
            newItem.Name = Name;
            newItem.PhoneNumber = PhoneNumber;
            newItem.StreetAddress = StreetAddress;
            newItem.EmailsToSendInventoryReportsTo = EmailsToSendInventoryReportsTo.Select(e => e).ToList();
            return newItem;
        }

		public Guid? AccountID {
			get;
			set;
		}
            
        public List<RestaurantInventorySection> InventorySections { get; set; }

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

        public List<EmailAddress> EmailsToSendInventoryReportsTo{ get; set; }
        public IEnumerable<EmailAddress> GetEmailsToSendInventoryReportsTo(){
            return EmailsToSendInventoryReportsTo;
        }

        /// <summary>
        /// If this is true, this is just here to support a user, but is not verified as a real restaurant
        /// </summary>
        public bool IsPlaceholder { get { return AccountID.HasValue == false; } }

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
                case MiseEventTypes.RestaurantReportingEmailSet:
                    WhenRestaurantReportingEmailSet((RestaurantReportingEmailSetEvent)entityEvent);
                    break;
                case MiseEventTypes.AccountCancelled:
                    WhenAccountCancelled((Mise.Core.Common.Events.Accounts.AccountCancelledEvent)entityEvent);
                    break;
                default:
                    throw new ArgumentException("Can't handle event " + entityEvent.EventType);
            }

            LastUpdatedDate = entityEvent.CreatedDate;
            Revision = entityEvent.EventOrder;
        }

        private void WhenAccountCancelled(Mise.Core.Common.Events.Accounts.AccountCancelledEvent ev)
        {
            if (ev != null)
            {
                AccountID = null;
            }
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

        private void WhenRestaurantReportingEmailSet(RestaurantReportingEmailSetEvent entityEvent){
            if (entityEvent.Email != null)
            {
                EmailsToSendInventoryReportsTo.Insert(0, entityEvent.Email);
            }
        }
    }
}
