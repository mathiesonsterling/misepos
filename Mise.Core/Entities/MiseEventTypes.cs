namespace Mise.Core.Entities
{
    /// <summary>
    /// Big list of every event that can happen in our system!  
    /// </summary>
    /// <remarks>Each of these should have a feature associated with them!</remarks>
    public enum MiseEventTypes
	{
		EmployeeRegistersRestaurant,

        CheckCreated,
        /// <summary>
        /// We created our check by swiping a card to open
        /// </summary>
        CheckCreatedWithCreditCard,
        CheckReopened,
        CheckSent,
        CustomerAssignedToCheck,
        
        OrderOnCheck,
        OrderItemDeleted,
        OrderItemModified,
        OrderItemSetMemo,
        OrderItemVoided,
        OrderItemWasted,

        EmployeeClockedIn,
        EmployeeClockedOut,
        InsufficientPermissions,
        BadLoginAttempt,
        NoSale,

        DiscountAppliedToCheck,
        DiscountRemovedFromCheck,

        MarkCheckAsPaid,
        CashPaidOnCheck,

        //TODO put these in order
        CreditCardAddedForPayment,
        CreditCardAuthorizationStarted,
        CreditCardAuthorized,
        CredtCardTipAddedToCharge,
        CreditCardCancelledEvent,
        CreditCardFailedAuthorization,
        CreditCardCloseRequested,
        CreditCardChargeCompleted,

        CompPaidDirectlyOnCheck,
        ItemCompedGeneral,
		ItemUncomped,

        EmployeeCreatedEvent,
		EmployeeLoggedIntoInventoryAppEvent,
		EmployeeRegisteredForInventoryAppEvent,
        EmployeeLoggedOutOfInventoryApp,

		EmployeeInvitedToApplication,
		EmployeeAcceptsInvitation,
		EmployeeRejectsInvitation,
		EmployeePasswordChanged,

        VendorCreatedEvent,
        VendorAddressUpdated,
        VendorPhoneNumberUpdated,
        RestaurantAssociatedWithVendor,
		VendorLineItemAdded,
		VendorRestaurantSetsPriceForReceivedItem,

        InventoryCreated,
        InventoryCompleted,
		InventorySectionCleared,
		InventorySectionCompleted,
		InventorySectionStartedByEmployee,
        InventoryLiquidItemMeasured,
        InventoryMadeCurrent,
        InventoryLineItemAdded,
		InventoryLineItemDeleted,
		InventoryLineItemMovedToNewPosition,
		InventoryNewSectionAdded,

        ReceivingOrderCreated,
		ReceivingOrderNoteAdded,
		ReceivingOrderCompleted,
		ReceivingOrderLineItemAdded,
		ReceivingOrderLineItemQuantityUpdated,
		ReceivingOrderLineItemZeroedOut,
		ReceivingOrderAssociatedWithPO,
		ReceivingOrderLineItemDeleted,

        PurchaseOrderCreated,
		PurchaseOrderLineItemAddedFromInventoryCalculation,
		PurchaseOrderLineItemAmountUpdated,
		PurchaseOrderApproved,
        PurchaseOrderSentToVendor,
		PurchaseOrderReceivedFromVendor,

        PARCreated,
		PARLineItemAdded,
		PARLineItemQuantityUpdated,
		ParLineItemDeleted,

        PlaceholderRestaurantCreated,
        InventorySectionAddedToRestaurant,
        NewRestaurantRegisteredOnApp,
		UserSelectedRestaurant,

        AccountRegisteredFromMobileDevice
    }
}
