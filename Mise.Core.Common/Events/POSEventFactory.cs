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
				EventOrder = GetNextEventID (),
				OrderItem = orderItem,
				CheckID = check.Id,
				EmployeeID = employee.Id,
			};
		}

		public OrderItemModifiedEvent CreateOrderItemModifiedEvent(OrderItem orderItem, IEnumerable<MenuItemModifier> mods,
			IEmployee employee, ICheck check){
			return new OrderItemModifiedEvent {
				EventOrder = GetNextEventID(),
				EmployeeID = employee.Id,
				CheckID = check.Id,
				Modifiers = mods,
				OrderItemID = orderItem.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public OrderItemSetMemoEvent CreateOrderItemSetMemoEvent(ICheck check, OrderItem orderItem, string memo, IEmployee employee){
			return new OrderItemSetMemoEvent {
                EventOrder = GetNextEventID(),
				CheckID = check.Id,
				Memo = memo,
				OrderItemID = orderItem.Id,
			    DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = employee.Id
			};
		}

		public CheckCreatedEvent CreateCheckCreatedEvent(IEmployee emp){
			return new CheckCreatedEvent {
				EventOrder = GetNextEventID (),
				CheckID = GetNextCheckID (),
				EmployeeID = emp.Id,
				RestaurantId = _restaurant.Id,
				DeviceId = _DeviceID,
				CreatedDate = DateTime.UtcNow,
			};
		}

	    public CheckCreatedWithCreditCardEvent CreateCheckCreatedWithCreditCardEvent(IEmployee emp, CreditCard card)
	    {
	        return new CheckCreatedWithCreditCardEvent
	        {
	            EventOrder = GetNextEventID(),
	            CheckID = GetNextCheckID(),
	            EmployeeID = emp.Id,
	            RestaurantId = _restaurant.Id,
	            DeviceId = _DeviceID,
	            CreatedDate = DateTime.UtcNow,
	            CreditCard = card
	        };
	    }
		public CheckSentEvent CreateCheckSentEvent(ICheck check, IEmployee emp){
			return new CheckSentEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = emp.Id,
				RestaurantId = _restaurant.Id,
			    DeviceId = _DeviceID,
				CreatedDate = DateTime.UtcNow
			};
		}

		public CustomerAssignedToCheckEvent CreateCustomerAssignedToTabEvent(ICheck check, CreditCard card){
			return new CustomerAssignedToCheckEvent{
                EventOrder = GetNextEventID(),
                Customer = new Customer (card),
				CheckID = check.Id,				
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public CustomerAssignedToCheckEvent CreateCustomerAssignedToTabEvent(Guid checkID, PersonName name, IEmployee employee){
			return new CustomerAssignedToCheckEvent
			{
				EventOrder = GetNextEventID(),
				Customer = new Customer{ Name = name } ,
				CheckID = checkID,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				EmployeeID = employee.Id,
			};
		}
			

		public CashPaidOnCheckEvent CreateCashPaidOnCheckEvent(ICheck tab, IEmployee emp, Money amountTendered, 
			Money amountPaid, Money change){

			return new CashPaidOnCheckEvent {
				EventOrder = GetNextEventID(),
				CheckID = tab.Id,
				EmployeeID = emp.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				AmountPaid = amountPaid,
				AmountTendered = amountTendered,
				ChangeGiven = change,
			};
		}
			
		public CompPaidDirectlyOnCheckEvent CreateCompPaidOnCheckEvent(ICheck tab, IEmployee emp, Money amt){
			return new CompPaidDirectlyOnCheckEvent {
				EventOrder = GetNextEventID(),
				CheckID = tab.Id,
				EmployeeID = emp.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				Amount = amt
			};
		}


		public ItemCompedGeneralEvent CreateItemCompedEvent(ICheck check, IEmployee emp, OrderItem orderItem,
			string reason){
			return new ItemCompedGeneralEvent {
                EventOrder = GetNextEventID(),
				CheckID = check.Id,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.Id,
				OrderItemID = orderItem.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				Reason = reason,
				Amount = orderItem.Total
			};
		}

		public ItemUncompedEvent CreateItemUncompedEvent(ICheck check, IEmployee Employee, OrderItem orderItem){
			return new ItemUncompedEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = Employee.Id,
				OrderItemID = orderItem.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				Amount = orderItem.Total
			};
		}
		public MarkCheckAsPaidEvent CreateMarkCheckAsPaidEvent(ICheck check, IEmployee emp){
			var isMultiple = check.GetPayments().Count()  > 1;
			return new MarkCheckAsPaidEvent {
				CheckID = check.Id,
                EventOrder = GetNextEventID(),
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.Id,
				IsSplitPayment = isMultiple,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public OrderItemVoidedEvent CreateOrderItemVoidedEvent(ICheck tab, IEmployee emp, IEmployee manager,
		                                                               OrderItem orderItem,string reason){
			return new OrderItemVoidedEvent {
				CheckID = tab.Id,
                EventOrder = GetNextEventID(),
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp != null ? emp.Id : Guid.Empty,
				ManagerApprovedID = manager.Id,
				OrderItemToVoid = orderItem,
				Reason = reason,
				ServerPlacedID = orderItem.PlacedByID,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				StatusWhenVoided = orderItem.Status,
			};
		}

		public OrderItemWastedEvent CreateOrderItemWastedEvent(ICheck tab, IEmployee emp, IEmployee manager,
			OrderItem orderItem,string reason){
			return new OrderItemWastedEvent {
				CheckID = tab.Id,
                EventOrder = GetNextEventID(),
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp != null ? emp.Id : Guid.Empty,
				ManagerApprovedID = manager.Id,
				OrderItemToVoid = orderItem,
				Reason = reason,
				ServerPlacedID = orderItem.PlacedByID,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				StatusWhenVoided = orderItem.Status,
			};
		}

		public BadLoginAttemptEvent CreateBadLoginEvent(string passcode, string functionName){
			return new BadLoginAttemptEvent { 
				EventOrder = GetNextEventID(),
				PasscodeGiven = passcode, 
				FunctionAttempted=functionName, 
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				CreatedDate = DateTime.UtcNow,
			};
		}

		public InsufficientPermissionsEvent CreateInsufficientPermissionEvent(IEmployee emp, string functionName){
			return new InsufficientPermissionsEvent {
				EventOrder = GetNextEventID (),
				FunctionAttempted = functionName,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				CreatedDate = DateTime.UtcNow,
				EmployeeID = emp.Id,
			};
		}
		public EmployeeClockedInEvent CreateEmployeeClockedInEvent(IEmployee thisEmp){
			return new EmployeeClockedInEvent{
                EventOrder = GetNextEventID(),
				EmployeeID = thisEmp.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public EmployeeClockedOutEvent CreateEmployeeClockedOutEvent(IEmployee thisEmp){
			return new EmployeeClockedOutEvent{ 
                EventOrder = GetNextEventID(),
				EmployeeID = thisEmp.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public NoSaleEvent CreateNoSaleEvent(IEmployee thisEmp){
			return new NoSaleEvent { 
                EventOrder = GetNextEventID(),
				EmployeeID = thisEmp.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public OrderItemDeletedEvent CreateOrderItemDeletedEvent(ICheck check, OrderItem orderItem, IEmployee emp){
			return new OrderItemDeletedEvent {
				EventOrder = GetNextEventID(),
				OrderItemID = orderItem.Id,
				EmployeeID = emp.Id,
				CheckID = check.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		public CheckReopenedEvent CreateReopenCheckEvent(ICheck check, IEmployee emp){
			return new CheckReopenedEvent {
				EventOrder = GetNextEventID(),
				EmployeeID = emp.Id,
				CheckID = check.Id,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id
			};
		}

		#region Credit Card Events
		public CreditCardAddedForPaymentEvent CreateCreditCardAddedForPaymentEvent(ICheck check, IEmployee employee,
			Money amount, CreditCard card){
			return new CreditCardAddedForPaymentEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				Amount = amount,
				CreatedDate = DateTime.UtcNow,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				CreditCard = card
			};
		}

	    public CreditCardAuthorizationStartedEvent CreateCreditCardAuthorizationStartedEvent(ICheck check,
	        IEmployee employee, ICreditCardPayment payment)
	    {
	        return new CreditCardAuthorizationStartedEvent
	        {
	            EventOrder = GetNextEventID(),
	            CheckID = check.Id,
	            EmployeeID = employee.Id,
	            PaymentID = payment.Id,
	        };
	    }

		public CreditCardAuthorizedEvent CreateCreditCardAuthorizedEvent(ICheck check, IEmployee employee, 
			ICreditCardPayment payment, CreditCardAuthorizationCode authCode){
			return new CreditCardAuthorizedEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				Amount = payment.AmountCharged,
				CreatedDate = DateTime.UtcNow,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				CreditCard = payment.Card,
				AuthorizationCode = authCode,
				PaymentID = payment.Id
			};
		}

		public CreditCardFailedAuthorizationEvent CreateCreditCardFailedAuthorizationEvent(ICheck check, 
			IEmployee employee, ICreditCardPayment payment, CreditCardAuthorizationCode authCode){
			return new CreditCardFailedAuthorizationEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				Amount = payment.AmountCharged,
				CreatedDate = DateTime.UtcNow,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
				CreditCard = payment.Card,
				AuthorizationCode = authCode,
				PaymentID = payment.Id
			};
		}

		public CreditCardChargeCompletedEvent CreateCreditCardCompletedEvent(ICheck check, IEmployee employee, 
			ICreditCardPayment payment, CreditCardAuthorizationCode authCode, bool successfulCharge){
			return new CreditCardChargeCompletedEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				AmountPaid = payment.AmountCharged,
				TipAmount = payment.TipAmount,
				AuthorizationCode = authCode,
				WasAuthorized = successfulCharge,
				CreditCard = payment.Card,
				PaymentID = payment.Id,
				RestaurantId = _restaurant.Id,
				DeviceId = _DeviceID
			};
		}
			
		public CreditCardCloseRequestedEvent CreateCreditCardCloseRequestedEvent(ICheck check, IEmployee employee,
			ICreditCardPayment payment){
			return new CreditCardCloseRequestedEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				AmountPaid = payment.AmountCharged,
				TipAmount = payment.TipAmount,
				PaymentID = payment.Id,
				CodeFromAuthorization = payment.AuthorizationResult,
				CreditCard = payment.Card,
				RestaurantId = _restaurant.Id,
				DeviceId = _DeviceID
			};
		}

		public CreditCardTipAddedToChargeEvent CreateCreditCardTipAddedToChargeEvent(ICheck check, IEmployee employee, ICreditCardPayment payment, Money tipAmount){
			return new CreditCardTipAddedToChargeEvent {
				EventOrder = GetNextEventID(),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				TipAmount = tipAmount,
				PaymentID = payment.Id,
				CreditCard = payment.Card,
				RestaurantId = _restaurant.Id,
				DeviceId = _DeviceID
			};
		}

		public CreditCardAuthorizationCancelledEvent CreditCardAuthorizationCancelled(ICheck check, IEmployee employee,
			ICreditCardPayment payment, CreditCardAuthorizationCode newAuthCode){
			return new CreditCardAuthorizationCancelledEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				AuthorizationCode = newAuthCode,
				WasRolledBack = newAuthCode.IsAuthorized,
				PaymentID = payment.Id
			};
		}
		#endregion

		public DiscountAppliedToCheckEvent CreateDiscountAppliedToCheckEvent(ICheck check, IEmployee employee, DiscountAmount discount){
			return new DiscountAppliedToCheckEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				DiscountAmount = discount,
				CreatedDate = DateTime.UtcNow,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
			};
		}
        public DiscountAppliedToCheckEvent CreateDiscountAppliedToCheckEvent(ICheck check, IEmployee employee, DiscountPercentage discount)
        {
            return new DiscountAppliedToCheckEvent
            {
                EventOrder = GetNextEventID(),
                CheckID = check.Id,
                EmployeeID = employee.Id,
                DiscountPercentage = discount,
                CreatedDate = DateTime.UtcNow,
                DeviceId = _DeviceID,
                RestaurantId = _restaurant.Id,
            };
        }
        public DiscountAppliedToCheckEvent CreateDiscountAppliedToCheckEvent(ICheck check, IEmployee employee, DiscountPercentageAfterMinimumCashTotal discount)
        {
            return new DiscountAppliedToCheckEvent
            {
                EventOrder = GetNextEventID(),
                CheckID = check.Id,
                EmployeeID = employee.Id,
                DiscountPercentageAfterMinimumCashTotal = discount,
                CreatedDate = DateTime.UtcNow,
                DeviceId = _DeviceID,
                RestaurantId = _restaurant.Id,
            };
        }
		public DiscountRemovedFromCheckEvent CreateDiscountRemovedFromCheckEvent(ICheck check, IEmployee employee, IDiscount discount){
			return new DiscountRemovedFromCheckEvent {
				EventOrder = GetNextEventID (),
				CheckID = check.Id,
				EmployeeID = employee.Id,
				DiscountID = discount.Id,
				CreatedDate = DateTime.UtcNow,
				DeviceId = _DeviceID,
				RestaurantId = _restaurant.Id,
			};
		}
	}
}

