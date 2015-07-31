using System;
using Mise.Core.Common.Events.Accounts;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Common.Events.Vendors;
using Mise.Core.Entities;
using Mise.Core.Entities.Accounts;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Entities.People.Events;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Entities.Vendors.Events;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Common.Events.DTOs
{
    /// <summary>
    /// Converts our events to and from DTOs.  Central clearinghouse for all event types!
    /// </summary>
    public class EventDataTransportObjectFactory
    {
        private readonly IJSONSerializer _jsonSerializer;

        public EventDataTransportObjectFactory(IJSONSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public EventDataTransportObject ToDataTransportObject(IEntityEventBase baseEvent)
        {
            if (baseEvent is EventDataTransportObject)
            {
                return baseEvent as EventDataTransportObject;
            }

            var checkEv = baseEvent as ICheckEvent;
            if (checkEv != null)
            {
                return ToDataTransportObject(checkEv);
            }

            var empEv = baseEvent as IEmployeeEvent;
            if (empEv != null)
            {
                return ToDataTransportObject(empEv);
            }

            var invEv = baseEvent as IInventoryEvent;
            if (invEv != null)
            {
                return ToDataTransportObject(invEv);
            }

            var purEv = baseEvent as IPurchaseOrderEvent;
            if (purEv != null)
            {
                return ToDataTransportObject(purEv);
            }

            var recEv = baseEvent as IReceivingOrderEvent;
            if (recEv != null)
            {
                return ToDataTransportObject(recEv);
            }

            var vendEv = baseEvent as IVendorEvent;
            if (vendEv != null)
            {
                return ToDataTransportObject(vendEv);
            }

            var parEv = baseEvent as IPAREvent;
            if (parEv != null)
            {
                return ToDataTransportObject(parEv);
            }

            var restEv = baseEvent as IRestaurantEvent;
            if (restEv != null)
            {
                return ToDataTransportObject(restEv);
            }

            var acctEv = baseEvent as IAccountEvent;
            if (acctEv != null)
            {
                return ToDataTransportObject(acctEv);
            }

			throw new ArgumentException ("Invalid event type of " + baseEvent.GetType ());
        }

        public EventDataTransportObject ToDataTransportObject(ICheckEvent checkEvent)
        {
            var json = _jsonSerializer.Serialize(checkEvent);
            return new EventDataTransportObject
            {
                EntityID = checkEvent.CheckID,
                CausedByID = checkEvent.CausedByID,
                CreatedDate = checkEvent.CreatedDate,
                EventOrderingID = checkEvent.EventOrderingID,
                EventType = checkEvent.EventType,
                ID = checkEvent.ID,
                JSON = json,
                RestaurantID = checkEvent.RestaurantID,
                SourceType = checkEvent.GetType()
            };
        }

        public EventDataTransportObject ToDataTransportObject(IEmployeeEvent empEvent)
        {
            var json = _jsonSerializer.Serialize(empEvent);
            return new EventDataTransportObject
            {
                EntityID = empEvent.EmployeeID,
                CausedByID = empEvent.CausedByID,
                CreatedDate = empEvent.CreatedDate,
                EventOrderingID = empEvent.EventOrderingID,
                EventType = empEvent.EventType,
                ID = empEvent.EmployeeID,
                JSON = json,
                RestaurantID = empEvent.RestaurantID,
                SourceType = empEvent.GetType()
            };
        }

        public EventDataTransportObject ToDataTransportObject(IInventoryEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
                DeviceID = ev.DeviceID,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				RestaurantID = ev.RestaurantID,
				EntityID = ev.InventoryID
			};
        }

        public EventDataTransportObject ToDataTransportObject(IPurchaseOrderEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				RestaurantID = ev.RestaurantID,
				EntityID = ev.PurchaseOrderID
			};
        }

        public EventDataTransportObject ToDataTransportObject(IVendorEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				EntityID = ev.VendorID
			};
        }

        public EventDataTransportObject ToDataTransportObject(IAccountEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				EntityID = ev.AccountID
			};
        }

        public EventDataTransportObject ToDataTransportObject(IReceivingOrderEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				RestaurantID = ev.RestaurantID,
				EntityID = ev.ReceivingOrderID
			};
        }

        public EventDataTransportObject ToDataTransportObject(IPAREvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				RestaurantID = ev.RestaurantID,
				EntityID = ev.ParID,
				SourceType = ev.GetType()
			};
        }

        public EventDataTransportObject ToDataTransportObject(IRestaurantEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				RestaurantID = ev.RestaurantID,
				EntityID = ev.RestaurantID
			};
        }

        public EventDataTransportObject ToDataTransportObject(IApplicationInvitationEvent ev)
        {
			var json = _jsonSerializer.Serialize(ev);
			return new EventDataTransportObject
			{
				CausedByID = ev.CausedByID,
				CreatedDate = ev.CreatedDate,
				EventOrderingID = ev.EventOrderingID,
				EventType = ev.EventType,
				ID = ev.ID,
				JSON = json,
				SourceType = ev.GetType(),
				RestaurantID = ev.RestaurantID,
				EntityID = ev.InvitationID
			};
        }

        /// <summary>
        /// Brute force attempts to deserialize - we'll want to improve this in the future
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEntityEventBase ToEvent(EventDataTransportObject source)
        {
            var checkEv = ToCheckEvent(source);
            if (checkEv != null)
            {
                return checkEv;
            }

            var empEvent = ToEmployeeEvent(source);
            if (empEvent != null)
            {
                return empEvent;
            }

            var invEvent = ToInventoryEvent(source);
            if (invEvent != null)
            {
                return invEvent;
            }

            var poEvent = ToPurchaseOrderEvent(source);
            if (poEvent != null)
            {
                return poEvent;
            }

            var roEvent = ToReceivingOrderEvent(source);
            if (roEvent != null)
            {
                return roEvent;
            }

            var vendorEvent = ToVendorEvent(source);
            if (vendorEvent != null)
            {
                return vendorEvent;
            }

            var parEvent = ToPAREvent(source);
            if (parEvent != null)
            {
                return parEvent;
            }

            var restEvent = ToRestaurantEvent(source);
            if (restEvent != null)
            {
                return restEvent;
            }

            var inviteEvent = ToApplicationInvitiationEvent(source);
            if (inviteEvent != null)
            {
                return inviteEvent;
            }

            var acctEvent = ToAccountEvent(source);
            if (acctEvent != null)
            {
                return acctEvent;
            }
            return null;
        }

        public IRestaurantEvent ToRestaurantEvent(EventDataTransportObject source)
        {
            switch (source.EventType)
            {
                case MiseEventTypes.PlaceholderRestaurantCreated:
                    return _jsonSerializer.Deserialize<PlaceholderRestaurantCreatedEvent>(source.JSON);
                case MiseEventTypes.InventorySectionAddedToRestaurant:
                    return _jsonSerializer.Deserialize<InventorySectionAddedToRestaurantEvent>(source.JSON);
				case MiseEventTypes.UserSelectedRestaurant:
					return _jsonSerializer.Deserialize<UserSelectedRestaurant>(source.JSON);
                default:
                    return null;
            }
        }

        public ICheckEvent ToCheckEvent(EventDataTransportObject source)
        {
            switch (source.EventType)
            {
                case MiseEventTypes.CashPaidOnCheck:
                    return _jsonSerializer.Deserialize<CashPaidOnCheckEvent>(source.JSON);
                case MiseEventTypes.CheckCreated:
                    return _jsonSerializer.Deserialize<CheckCreatedEvent>(source.JSON);
                case MiseEventTypes.CheckCreatedWithCreditCard:
                    return _jsonSerializer.Deserialize<CheckCreatedWithCreditCardEvent>(source.JSON);
                case MiseEventTypes.CheckReopened:
                    return _jsonSerializer.Deserialize<CheckReopenedEvent>(source.JSON);
                case MiseEventTypes.CheckSent:
                    return _jsonSerializer.Deserialize<CheckSentEvent>(source.JSON);
                case MiseEventTypes.CompPaidDirectlyOnCheck:
                    return _jsonSerializer.Deserialize<CompPaidDirectlyOnCheckEvent>(source.JSON);
                case MiseEventTypes.CreditCardAddedForPayment:
                    return _jsonSerializer.Deserialize<CreditCardAddedForPaymentEvent>(source.JSON);
                case MiseEventTypes.CreditCardAuthorizationStarted:
                    return _jsonSerializer.Deserialize<CreditCardAuthorizationStartedEvent>(source.JSON);
                case MiseEventTypes.CreditCardAuthorized:
                    return _jsonSerializer.Deserialize<CreditCardAuthorizedEvent>(source.JSON);
                case MiseEventTypes.CreditCardCancelledEvent:
                    return _jsonSerializer.Deserialize<CreditCardAuthorizationCancelledEvent>(source.JSON);
                case MiseEventTypes.CreditCardChargeCompleted:
                    return _jsonSerializer.Deserialize<CreditCardChargeCompletedEvent>(source.JSON);
                case MiseEventTypes.CreditCardCloseRequested:
                    return _jsonSerializer.Deserialize<CreditCardCloseRequestedEvent>(source.JSON);
                case MiseEventTypes.CreditCardFailedAuthorization:
                    return _jsonSerializer.Deserialize<CreditCardFailedAuthorizationEvent>(source.JSON);
                case MiseEventTypes.CredtCardTipAddedToCharge:
                    return _jsonSerializer.Deserialize<CreditCardTipAddedToChargeEvent>(source.JSON);
                case MiseEventTypes.CustomerAssignedToCheck:
                    return _jsonSerializer.Deserialize<CustomerAssignedToCheckEvent>(source.JSON);
                case MiseEventTypes.DiscountAppliedToCheck:
                    return _jsonSerializer.Deserialize<DiscountAppliedToCheckEvent>(source.JSON);
                case MiseEventTypes.DiscountRemovedFromCheck:
                    return _jsonSerializer.Deserialize<DiscountRemovedFromCheckEvent>(source.JSON);
                case MiseEventTypes.ItemCompedGeneral:
                    return _jsonSerializer.Deserialize<ItemCompedGeneralEvent>(source.JSON);
                case MiseEventTypes.ItemUncomped:
                    return _jsonSerializer.Deserialize<ItemUncompedEvent>(source.JSON);
                case MiseEventTypes.MarkCheckAsPaid:
                    return _jsonSerializer.Deserialize<MarkCheckAsPaidEvent>(source.JSON);
                case MiseEventTypes.OrderItemDeleted:
                    return _jsonSerializer.Deserialize<OrderItemDeletedEvent>(source.JSON);
                case MiseEventTypes.OrderItemModified:
                    return _jsonSerializer.Deserialize<OrderItemModifiedEvent>(source.JSON);
                case MiseEventTypes.OrderItemSetMemo:
                    return _jsonSerializer.Deserialize<OrderItemSetMemoEvent>(source.JSON);
                case MiseEventTypes.OrderItemVoided:
                    return _jsonSerializer.Deserialize<OrderItemVoidedEvent>(source.JSON);
                case MiseEventTypes.OrderItemWasted:
                    return _jsonSerializer.Deserialize<OrderItemWastedEvent>(source.JSON);
                case MiseEventTypes.OrderOnCheck:
                    return _jsonSerializer.Deserialize<OrderedOnCheckEvent>(source.JSON);
                default:
                    return null;

            }
        }

        public IEmployeeEvent ToEmployeeEvent(EventDataTransportObject source)
        {
            switch (source.EventType)
            {
                case MiseEventTypes.EmployeeCreatedEvent:
                    return _jsonSerializer.Deserialize<EmployeeCreatedEvent>(source.JSON);

                case MiseEventTypes.BadLoginAttempt:
                    return _jsonSerializer.Deserialize<BadLoginAttemptEvent>(source.JSON);
                case MiseEventTypes.CompPaidDirectlyOnCheck:
                    return _jsonSerializer.Deserialize<CompPaidDirectlyOnCheckEvent>(source.JSON);
                case MiseEventTypes.EmployeeClockedIn:
                    return _jsonSerializer.Deserialize<EmployeeClockedInEvent>(source.JSON);
                case MiseEventTypes.EmployeeClockedOut:
                    return _jsonSerializer.Deserialize<EmployeeClockedOutEvent>(source.JSON);
                case MiseEventTypes.InsufficientPermissions:
                    return _jsonSerializer.Deserialize<InsufficientPermissionsEvent>(source.JSON);
                case MiseEventTypes.ItemCompedGeneral:
                    return _jsonSerializer.Deserialize<ItemCompedGeneralEvent>(source.JSON);
                case MiseEventTypes.ItemUncomped:
                    return _jsonSerializer.Deserialize<ItemUncompedEvent>(source.JSON);
                case MiseEventTypes.NoSale:
                    return _jsonSerializer.Deserialize<NoSaleEvent>(source.JSON);


                case MiseEventTypes.EmployeeLoggedIntoInventoryAppEvent:
                    return _jsonSerializer.Deserialize<EmployeeLoggedIntoInventoryAppEvent>(source.JSON);
                case MiseEventTypes.EmployeeLoggedOutOfInventoryApp:
                    return _jsonSerializer.Deserialize<EmployeeLoggedOutOfInventoryAppEvent>(source.JSON);
                case MiseEventTypes.EmployeeRegisteredForInventoryAppEvent:
                    return _jsonSerializer.Deserialize<EmployeeRegisteredForInventoryAppEvent>(source.JSON);
                case MiseEventTypes.EmployeeAcceptsInvitation:
                    return _jsonSerializer.Deserialize<EmployeeAcceptsInvitationEvent>(source.JSON);
                case MiseEventTypes.EmployeeRejectsInvitation:
                    return _jsonSerializer.Deserialize<EmployeeRejectsInvitationEvent>(source.JSON);
                default:
                    return null;

            }
        }

        public IInventoryEvent ToInventoryEvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.InventoryCreated:
                    return _jsonSerializer.Deserialize<InventoryCreatedEvent>(dto.JSON);
                case MiseEventTypes.InventoryCompleted:
                    return _jsonSerializer.Deserialize<InventoryCompletedEvent>(dto.JSON);
                case MiseEventTypes.InventoryLiquidItemMeasured:
                    return _jsonSerializer.Deserialize<InventoryLiquidItemMeasuredEvent>(dto.JSON);
                case MiseEventTypes.InventoryLineItemAdded:
                    return _jsonSerializer.Deserialize<InventoryLineItemAddedEvent>(dto.JSON);
                case MiseEventTypes.InventoryMadeCurrent:
                    return _jsonSerializer.Deserialize<InventoryMadeCurrentEvent>(dto.JSON);
                case MiseEventTypes.InventorySectionCompleted:
                    return _jsonSerializer.Deserialize<InventorySectionCompletedEvent>(dto.JSON);
                case MiseEventTypes.InventoryNewSectionAdded:
                    return _jsonSerializer.Deserialize<InventoryNewSectionAddedEvent>(dto.JSON);
                default:
                    return null;
            }
        }

        public IApplicationInvitationEvent ToApplicationInvitiationEvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.EmployeeInvitedToApplication:
                    return _jsonSerializer.Deserialize<EmployeeInvitedToApplicationEvent>(dto.JSON);
                case MiseEventTypes.EmployeeAcceptsInvitation:
                    return _jsonSerializer.Deserialize<EmployeeAcceptsInvitationEvent>(dto.JSON);
                case MiseEventTypes.EmployeeRejectsInvitation:
                    return _jsonSerializer.Deserialize<EmployeeRejectsInvitationEvent>(dto.JSON);
                default:
                    return null;
            }
        }

        public IPurchaseOrderEvent ToPurchaseOrderEvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.PurchaseOrderCreated:
                    return _jsonSerializer.Deserialize<PurchaseOrderCreatedEvent>(dto.JSON);
			    case MiseEventTypes.PurchaseOrderLineItemAddedFromInventoryCalculation:
				    return _jsonSerializer.Deserialize<PurchaseOrderLineItemAddedFromInventoryCalculationEvent> (dto.JSON);
                case MiseEventTypes.PurchaseOrderSentToVendor:
                    return _jsonSerializer.Deserialize<PurchaseOrderSentToVendorEvent>(dto.JSON);
                case MiseEventTypes.PurchaseOrderReceivedFromVendor:
                    return _jsonSerializer.Deserialize<PurchaseOrderRecievedFromVendorEvent>(dto.JSON);
                case MiseEventTypes.PurchaseOrderApproved:
                case MiseEventTypes.PurchaseOrderLineItemAmountUpdated:
                    throw new NotImplementedException("Cannot approve or update PO amounts currently!");
                default:
                    return null;
            }
        }

        public IReceivingOrderEvent ToReceivingOrderEvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.ReceivingOrderCreated:
                    return _jsonSerializer.Deserialize<ReceivingOrderCreatedEvent>(dto.JSON);
                case MiseEventTypes.ReceivingOrderCompleted:
                    return _jsonSerializer.Deserialize<ReceivingOrderCompletedEvent>(dto.JSON);
                case MiseEventTypes.ReceivingOrderLineItemAdded:
                    return _jsonSerializer.Deserialize<ReceivingOrderLineItemAddedEvent>(dto.JSON);
                case MiseEventTypes.ReceivingOrderNoteAdded:
                    return _jsonSerializer.Deserialize<ReceivingOrderNoteAddedEvent>(dto.JSON);
                case MiseEventTypes.ReceivingOrderLineItemQuantityUpdated:
                    return _jsonSerializer.Deserialize<ReceivingOrderLineItemQuantityUpdatedEvent>(dto.JSON);
                case MiseEventTypes.ReceivingOrderLineItemZeroedOut:
                    return _jsonSerializer.Deserialize<ReceivingOrderLineItemZeroedOutEvent>(dto.JSON);
                case MiseEventTypes.ReceivingOrderAssociatedWithPO:
                    return _jsonSerializer.Deserialize<ReceivingOrderAssociatedWithPOEvent>(dto.JSON);
                default:
                    return null;
            }
        }

        public IVendorEvent ToVendorEvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.VendorCreatedEvent:
                    return _jsonSerializer.Deserialize<VendorCreatedEvent>(dto.JSON);
                case MiseEventTypes.VendorAddressUpdated:
                    return _jsonSerializer.Deserialize<VendorAddressUpdatedEvent>(dto.JSON);
                case MiseEventTypes.VendorPhoneNumberUpdated:
                    return _jsonSerializer.Deserialize<VendorPhoneNumberUpdatedEvent>(dto.JSON);
                case MiseEventTypes.RestaurantAssociatedWithVendor:
                    return _jsonSerializer.Deserialize<RestaurantAssociatedWithVendorEvent>(dto.JSON);
                case MiseEventTypes.VendorLineItemAdded:
                    return _jsonSerializer.Deserialize<VendorAddNewLineItemEvent>(dto.JSON);
                case MiseEventTypes.VendorRestaurantSetsPriceForReceivedItem:
                    return _jsonSerializer.Deserialize<VendorRestaurantSetsPriceForReceivedItemEvent>(dto.JSON);
                default:
                    return null;
            }
        }

        public IPAREvent ToPAREvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.PARCreated:
                    return _jsonSerializer.Deserialize<PARCreatedEvent>(dto.JSON);
                case MiseEventTypes.PARLineItemAdded:
                    return _jsonSerializer.Deserialize<PARLineItemAddedEvent>(dto.JSON);
                case MiseEventTypes.PARLineItemQuantityUpdated:
                    return _jsonSerializer.Deserialize<PARLineItemQuantityUpdatedEvent>(dto.JSON);
                default:
                    return null;
            }
        }

        public IAccountEvent ToAccountEvent(EventDataTransportObject dto)
        {
            switch (dto.EventType)
            {
                case MiseEventTypes.AccountRegisteredFromMobileDevice:
                    return _jsonSerializer.Deserialize<AccountRegisteredFromMobileDeviceEvent>(dto.JSON);
                default:
                    return null;
            }
        }
    }
}
