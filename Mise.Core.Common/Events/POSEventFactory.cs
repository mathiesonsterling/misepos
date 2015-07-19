using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
using Mise.Core.ValueItems;
using Mise.Core.Entities;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Entities.Payments;


namespace Mise.Core.Common.Events
{
	/// <summary>
	/// Factory class for events that are used by the POS
	/// </summary>
	public class POSEventFactory
	{
		long LastEventID;

		readonly IRestaurant _restaurant;
		readonly string _DeviceID;
	    readonly MiseAppTypes _appCode;

		public POSEventFactory(IMiseTerminalDevice termSettings, IRestaurant restaurant){
			_restaurant = restaurant;
			_DeviceID = termSettings.MachineID;
		    _appCode = termSettings.Application;
		}

		static Guid GetNextCheckID()
		{
			return Guid.NewGuid ();
		}

		EventID GetNextEventID(){
			var orderingID = (LastEventID++);
			if(orderingID == long.MaxValue){
				orderingID = 1;
			}
		    return new EventID {AppInstanceCode = _appCode, OrderingID = orderingID};
		}

		public OrderedOnCheckEvent CreateOrderedOnCheckEvent(OrderItem orderItem, IEmployee employee, ICheck check){
			return new OrderedOnCheckEvent{
				EventOrderingID = GetNextEventID (),
				OrderItem = orderItem,
				CheckID = check.ID,
				EmployeeID = employee.ID,
			};
		}

		public OrderItemModifiedEvent CreateOrderItemModifiedEvent(OrderItem orderItem, IEnumerable<MenuItemModifier> mods,
			IEmployee employee, ICheck check){
			return new OrderItemModifiedEvent {
				EventOrderingID = GetNextEventID(),
				EmployeeID = employee.ID,
				CheckID = check.ID,
				Modifiers = mods,
				OrderItemID = orderItem.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public OrderItemSetMemoEvent CreateOrderItemSetMemoEvent(ICheck check, OrderItem orderItem, string memo, IEmployee employee){
			return new OrderItemSetMemoEvent {
                EventOrderingID = GetNextEventID(),
				CheckID = check.ID,
				Memo = memo,
				OrderItemID = orderItem.ID,
			    DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = employee.ID
			};
		}

		public CheckCreatedEvent CreateCheckCreatedEvent(IEmployee emp){
			return new CheckCreatedEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = GetNextCheckID (),
				EmployeeID = emp.ID,
				RestaurantID = _restaurant.ID,
				DeviceID = _DeviceID,
				CreatedDate = DateTime.UtcNow,
			};
		}

	    public CheckCreatedWithCreditCardEvent CreateCheckCreatedWithCreditCardEvent(IEmployee emp, CreditCard card)
	    {
	        return new CheckCreatedWithCreditCardEvent
	        {
	            EventOrderingID = GetNextEventID(),
	            CheckID = GetNextCheckID(),
	            EmployeeID = emp.ID,
	            RestaurantID = _restaurant.ID,
	            DeviceID = _DeviceID,
	            CreatedDate = DateTime.UtcNow,
	            CreditCard = card
	        };
	    }
		public CheckSentEvent CreateCheckSentEvent(ICheck check, IEmployee emp){
			return new CheckSentEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = emp.ID,
				RestaurantID = _restaurant.ID,
			    DeviceID = _DeviceID,
				CreatedDate = DateTime.UtcNow
			};
		}

		public CustomerAssignedToCheckEvent CreateCustomerAssignedToTabEvent(ICheck check, CreditCard card){
			return new CustomerAssignedToCheckEvent{
                EventOrderingID = GetNextEventID(),
                Customer = new Customer (card),
				CheckID = check.ID,				
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public CustomerAssignedToCheckEvent CreateCustomerAssignedToTabEvent(Guid checkID, PersonName name, IEmployee employee){
			return new CustomerAssignedToCheckEvent
			{
				EventOrderingID = GetNextEventID(),
				Customer = new Customer{ Name = name } ,
				CheckID = checkID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				EmployeeID = employee.ID,
			};
		}
			

		public CashPaidOnCheckEvent CreateCashPaidOnCheckEvent(ICheck tab, IEmployee emp, Money amountTendered, 
			Money amountPaid, Money change){

			return new CashPaidOnCheckEvent {
				EventOrderingID = GetNextEventID(),
				CheckID = tab.ID,
				EmployeeID = emp.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				AmountPaid = amountPaid,
				AmountTendered = amountTendered,
				ChangeGiven = change,
			};
		}
			
		public CompPaidDirectlyOnCheckEvent CreateCompPaidOnCheckEvent(ICheck tab, IEmployee emp, Money amt){
			return new CompPaidDirectlyOnCheckEvent {
				EventOrderingID = GetNextEventID(),
				CheckID = tab.ID,
				EmployeeID = emp.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				Amount = amt
			};
		}


		public ItemCompedGeneralEvent CreateItemCompedEvent(ICheck check, IEmployee emp, OrderItem orderItem,
			string reason){
			return new ItemCompedGeneralEvent {
                EventOrderingID = GetNextEventID(),
				CheckID = check.ID,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.ID,
				OrderItemID = orderItem.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				Reason = reason,
				Amount = orderItem.Total
			};
		}

		public ItemUncompedEvent CreateItemUncompedEvent(ICheck check, IEmployee Employee, OrderItem orderItem){
			return new ItemUncompedEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = Employee.ID,
				OrderItemID = orderItem.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				Amount = orderItem.Total
			};
		}
		public MarkCheckAsPaidEvent CreateMarkCheckAsPaidEvent(ICheck check, IEmployee emp){
			var isMultiple = check.GetPayments().Count()  > 1;
			return new MarkCheckAsPaidEvent {
				CheckID = check.ID,
                EventOrderingID = GetNextEventID(),
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.ID,
				IsSplitPayment = isMultiple,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public OrderItemVoidedEvent CreateOrderItemVoidedEvent(ICheck tab, IEmployee emp, IEmployee manager,
		                                                               OrderItem orderItem,string reason){
			return new OrderItemVoidedEvent {
				CheckID = tab.ID,
                EventOrderingID = GetNextEventID(),
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp != null ? emp.ID : Guid.Empty,
				ManagerApprovedID = manager.ID,
				OrderItemToVoid = orderItem,
				Reason = reason,
				ServerPlacedID = orderItem.PlacedByID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				StatusWhenVoided = orderItem.Status,
			};
		}

		public OrderItemWastedEvent CreateOrderItemWastedEvent(ICheck tab, IEmployee emp, IEmployee manager,
			OrderItem orderItem,string reason){
			return new OrderItemWastedEvent {
				CheckID = tab.ID,
                EventOrderingID = GetNextEventID(),
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp != null ? emp.ID : Guid.Empty,
				ManagerApprovedID = manager.ID,
				OrderItemToVoid = orderItem,
				Reason = reason,
				ServerPlacedID = orderItem.PlacedByID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				StatusWhenVoided = orderItem.Status,
			};
		}

		public BadLoginAttemptEvent CreateBadLoginEvent(string passcode, string functionName){
			return new BadLoginAttemptEvent { 
				EventOrderingID = GetNextEventID(),
				PasscodeGiven = passcode, 
				FunctionAttempted=functionName, 
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				CreatedDate = DateTime.UtcNow,
			};
		}

		public InsufficientPermissionsEvent CreateInsufficientPermissionEvent(IEmployee emp, string functionName){
			return new InsufficientPermissionsEvent {
				EventOrderingID = GetNextEventID (),
				FunctionAttempted = functionName,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.ID,
			};
		}
		public EmployeeClockedInEvent CreateEmployeeClockedInEvent(IEmployee thisEmp){
			return new EmployeeClockedInEvent{
                EventOrderingID = GetNextEventID(),
				EmployeeID = thisEmp.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public EmployeeClockedOutEvent CreateEmployeeClockedOutEvent(IEmployee thisEmp){
			return new EmployeeClockedOutEvent{ 
                EventOrderingID = GetNextEventID(),
				EmployeeID = thisEmp.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public NoSaleEvent CreateNoSaleEvent(IEmployee thisEmp){
			return new NoSaleEvent { 
                EventOrderingID = GetNextEventID(),
				EmployeeID = thisEmp.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public OrderItemDeletedEvent CreateOrderItemDeletedEvent(ICheck check, OrderItem orderItem, IEmployee emp){
			return new OrderItemDeletedEvent {
				EventOrderingID = GetNextEventID(),
				OrderItemID = orderItem.ID,
				EmployeeID = emp.ID,
				CheckID = check.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		public CheckReopenedEvent CreateReopenCheckEvent(ICheck check, IEmployee emp){
			return new CheckReopenedEvent {
				EventOrderingID = GetNextEventID(),
				EmployeeID = emp.ID,
				CheckID = check.ID,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID
			};
		}

		#region Credit Card Events
		public CreditCardAddedForPaymentEvent CreateCreditCardAddedForPaymentEvent(ICheck check, IEmployee employee,
			Money amount, CreditCard card){
			return new CreditCardAddedForPaymentEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				Amount = amount,
				CreatedDate = DateTime.UtcNow,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				CreditCard = card
			};
		}

	    public CreditCardAuthorizationStartedEvent CreateCreditCardAuthorizationStartedEvent(ICheck check,
	        IEmployee employee, ICreditCardPayment payment)
	    {
	        return new CreditCardAuthorizationStartedEvent
	        {
	            EventOrderingID = GetNextEventID(),
	            CheckID = check.ID,
	            EmployeeID = employee.ID,
	            PaymentID = payment.ID,
	        };
	    }

		public CreditCardAuthorizedEvent CreateCreditCardAuthorizedEvent(ICheck check, IEmployee employee, 
			ICreditCardPayment payment, CreditCardAuthorizationCode authCode){
			return new CreditCardAuthorizedEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				Amount = payment.AmountCharged,
				CreatedDate = DateTime.UtcNow,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				CreditCard = payment.Card,
				AuthorizationCode = authCode,
				PaymentID = payment.ID
			};
		}

		public CreditCardFailedAuthorizationEvent CreateCreditCardFailedAuthorizationEvent(ICheck check, 
			IEmployee employee, ICreditCardPayment payment, CreditCardAuthorizationCode authCode){
			return new CreditCardFailedAuthorizationEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				Amount = payment.AmountCharged,
				CreatedDate = DateTime.UtcNow,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
				CreditCard = payment.Card,
				AuthorizationCode = authCode,
				PaymentID = payment.ID
			};
		}

		public CreditCardChargeCompletedEvent CreateCreditCardCompletedEvent(ICheck check, IEmployee employee, 
			ICreditCardPayment payment, CreditCardAuthorizationCode authCode, bool successfulCharge){
			return new CreditCardChargeCompletedEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				AmountPaid = payment.AmountCharged,
				TipAmount = payment.TipAmount,
				AuthorizationCode = authCode,
				WasAuthorized = successfulCharge,
				CreditCard = payment.Card,
				PaymentID = payment.ID,
				RestaurantID = _restaurant.ID,
				DeviceID = _DeviceID
			};
		}
			
		public CreditCardCloseRequestedEvent CreateCreditCardCloseRequestedEvent(ICheck check, IEmployee employee,
			ICreditCardPayment payment){
			return new CreditCardCloseRequestedEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				AmountPaid = payment.AmountCharged,
				TipAmount = payment.TipAmount,
				PaymentID = payment.ID,
				CodeFromAuthorization = payment.AuthorizationResult,
				CreditCard = payment.Card,
				RestaurantID = _restaurant.ID,
				DeviceID = _DeviceID
			};
		}

		public CreditCardTipAddedToChargeEvent CreateCreditCardTipAddedToChargeEvent(ICheck check, IEmployee employee, ICreditCardPayment payment, Money tipAmount){
			return new CreditCardTipAddedToChargeEvent {
				EventOrderingID = GetNextEventID(),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				TipAmount = tipAmount,
				PaymentID = payment.ID,
				CreditCard = payment.Card,
				RestaurantID = _restaurant.ID,
				DeviceID = _DeviceID
			};
		}

		public CreditCardAuthorizationCancelledEvent CreditCardAuthorizationCancelled(ICheck check, IEmployee employee,
			ICreditCardPayment payment, CreditCardAuthorizationCode newAuthCode){
			return new CreditCardAuthorizationCancelledEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				AuthorizationCode = newAuthCode,
				WasRolledBack = newAuthCode.IsAuthorized,
				PaymentID = payment.ID
			};
		}
		#endregion

		public DiscountAppliedToCheckEvent CreateDiscountAppliedToCheckEvent(ICheck check, IEmployee employee, DiscountAmount discount){
			return new DiscountAppliedToCheckEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				DiscountAmount = discount,
				CreatedDate = DateTime.UtcNow,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
			};
		}
        public DiscountAppliedToCheckEvent CreateDiscountAppliedToCheckEvent(ICheck check, IEmployee employee, DiscountPercentage discount)
        {
            return new DiscountAppliedToCheckEvent
            {
                EventOrderingID = GetNextEventID(),
                CheckID = check.ID,
                EmployeeID = employee.ID,
                DiscountPercentage = discount,
                CreatedDate = DateTime.UtcNow,
                DeviceID = _DeviceID,
                RestaurantID = _restaurant.ID,
            };
        }
        public DiscountAppliedToCheckEvent CreateDiscountAppliedToCheckEvent(ICheck check, IEmployee employee, DiscountPercentageAfterMinimumCashTotal discount)
        {
            return new DiscountAppliedToCheckEvent
            {
                EventOrderingID = GetNextEventID(),
                CheckID = check.ID,
                EmployeeID = employee.ID,
                DiscountPercentageAfterMinimumCashTotal = discount,
                CreatedDate = DateTime.UtcNow,
                DeviceID = _DeviceID,
                RestaurantID = _restaurant.ID,
            };
        }
		public DiscountRemovedFromCheckEvent CreateDiscountRemovedFromCheckEvent(ICheck check, IEmployee employee, IDiscount discount){
			return new DiscountRemovedFromCheckEvent {
				EventOrderingID = GetNextEventID (),
				CheckID = check.ID,
				EmployeeID = employee.ID,
				DiscountID = discount.ID,
				CreatedDate = DateTime.UtcNow,
				DeviceID = _DeviceID,
				RestaurantID = _restaurant.ID,
			};
		}
	}
}

