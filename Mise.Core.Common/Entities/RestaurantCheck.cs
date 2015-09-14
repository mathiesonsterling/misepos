using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Common.Events.Payments;
using Mise.Core.Common.Events.Payments.CreditCards;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Payments;


namespace Mise.Core.Common.Entities
{
    /// <summary>
    /// A Check for people at a bar, that don't have sections or table seating, but do allow you to order more than once on it
    /// </summary>
    public class RestaurantCheck : RestaurantEntityBase, ICheck, IEventStoreEntityBase<ICheckEvent>
    {
        public RestaurantCheck()
        {
            _orderItems = new List<OrderItem>();
            CreditCards = new List<CreditCard>();
            ChangeDue = Money.None;

            CashPayments = new List<CashPayment>();
            CreditCardPayments = new List<CreditCardPayment>();
            CompAmountPayments = new List<CompAmountPayment>();
            CompItemsPayments = new List<CompItemPayment>();

            DiscountAmounts= new List<DiscountAmount>();
            DiscountPercentages = new List<DiscountPercentage>();
            DiscountPercentageAfterMinimumCashTotals = new List<DiscountPercentageAfterMinimumCashTotal>();
        }

        public void UpdatePaymentStatusFromPayments()
        {
            //do we have rejected ones?
            var payments = GetPayments().ToList();
            if (payments.Any() == false)
            {
                //no change yet possible!
                return;
            }

            var ccPayments = payments.OfType<IProcessingPayment>().ToList();
            //check for created payments, this is an error!
            var createdButNotStartedPayments =
                ccPayments.Any(c => c.PaymentProcessingStatus == PaymentProcessingStatus.Created);
            if (createdButNotStartedPayments)
            {
                throw new ArgumentException("Cannot apply MarkCheckAsPaidEvent with Created processing payments!");
            }

            var processingPayments =
                ccPayments.Where(p => p.PaymentProcessingStatus == PaymentProcessingStatus.SentForBaseAuthorization);
            if (processingPayments.Any())
            {
                PaymentStatus =  CheckPaymentStatus.PaymentPending;
                return;
            }

            var rejected =
                ccPayments.Where(
                    c =>
                        c.PaymentProcessingStatus == PaymentProcessingStatus.BaseRejected ||
                        c.PaymentProcessingStatus == PaymentProcessingStatus.FullAmountRejected)
                        .ToList();

            var validPayments = payments.Except(rejected);

            var totalPaid = validPayments.Aggregate(Money.None, (c, payment) => c.Add(payment.AmountToApplyToCheckTotal));
            if (totalPaid.GreaterThanOrEqualTo(Total))
            {
                //do we have stuff waiting for tips?
                if (ccPayments.Any(c => c.PaymentProcessingStatus == PaymentProcessingStatus.BaseAuthorized))
                {
                    PaymentStatus =CheckPaymentStatus.PaymentApprovedWithoutTip;
                    return;
                }
                ClosedDate = DateTime.UtcNow;
                PaymentStatus = CheckPaymentStatus.Closed;
                return;
            }
            //we're either rejected or open
            if (rejected.Any())
            {
                PaymentStatus = CheckPaymentStatus.PaymentRejected;
            }

        }

        public ICloneableEntity Clone()
        {
			var newRestaurantCheck = CloneRestaurantBase(new RestaurantCheck
            {
                CreatedByServerID = CreatedByServerID,
                LastTouchedServerID = LastTouchedServerID,
                Customer = Customer,
                PaymentStatus = PaymentStatus,
                ClosedDate = ClosedDate,
            });

            foreach (var oi in OrderItems)
            {
                newRestaurantCheck.AddOrderItem(oi.Clone());
            }

            foreach (var pay in GetPayments())
            {
                newRestaurantCheck.AddPayment(pay.Clone());
            }
            return newRestaurantCheck;
        }

        private List<OrderItem> _orderItems;

        public IEnumerable<OrderItem> OrderItems
        {
            get { return _orderItems; }
            set { _orderItems = value.ToList(); }
        }

        public void AddOrderItem(OrderItem item)
        {
            _orderItems.Add(item);
        }

        public void RemoveOrderItem(OrderItem item)
        {
            _orderItems.Remove(item);
        }

        public Money Total
        {
            get
            {
                var total = OrderItems.Where(oi => oi.EmployeeWhoComped.HasValue == false)
                    .Aggregate(Money.None, (current, oi) => current.Add(oi.Total));

                //ois know how to add their own mods

                //apply any discounts we have
                var totalDiscountAmount = GetDiscounts().Aggregate(Money.None,
                    (current, d) => current.Add(d.GetDiscountAmount(total)));
                return total.Add(totalDiscountAmount);
            }
        }

        /// <summary>
        /// Amount of change total from all payments
        /// </summary>
        /// <value>The change due.</value>
        public Money ChangeDue { get; set; }

        /// <summary>
        /// ID of the server that is started this check
        /// </summary>
        public Guid CreatedByServerID { get; set; }

        /// <summary>
        /// Last employee that touched this check
        /// </summary>
        /// <value>The last touched server I.</value>
        public Guid LastTouchedServerID { get; set; }

        public Customer Customer { get; set; }

        public CheckPaymentStatus PaymentStatus { get; set; }


        public DateTime? ClosedDate { get; set; }


        public string DisplayName
        {
            get { return (GetTopOfCheck() + "  " + CreatedDate.ToString("H:mm")); }
        }


        public string GetTopOfCheck()
        {
            if (Customer == null || Customer.Name == null)
            {
                return string.Empty;
            }
            return Customer.Name.FirstName + " " + Customer.Name.LastName;
        }

        private IList<CreditCard> _creditCards;

        public IEnumerable<CreditCard> CreditCards
        {
            get { return _creditCards; }
            set { _creditCards = value.ToList(); }
        }

        private void AddCreditCard(CreditCard card)
        {
            if (_creditCards.Select(cc => cc.ProcessorToken.Token).Contains(card.ProcessorToken.Token) == false)
            {
                _creditCards.Add(card);
            }
        }

        #region Payments
        public List<CashPayment> CashPayments { get; set; }
        public List<CreditCardPayment> CreditCardPayments { get; set; }

        public List<CompAmountPayment> CompAmountPayments { get; set; }

        public List<CompItemPayment> CompItemsPayments { get; set; } 

        public virtual IEnumerable<IPayment> GetPayments()
        {
            var list = new List<IPayment>();
            if (CashPayments != null)
            {
                list.AddRange(CashPayments);
            }
            if (CreditCardPayments != null)
            {
                list.AddRange(CreditCardPayments);
            }
            if (CompAmountPayments != null)
            {
                list.AddRange(CompAmountPayments);
            }
            if (CompItemsPayments != null)
            {
                list.AddRange(CompItemsPayments);
            }
            return list;
        }


        public virtual void AddPayment(IPayment payment)
        {
            //switch on the type to put in the right list 
            var cashPayment = payment as CashPayment;
            if (cashPayment != null)
            {
                CashPayments.Add(cashPayment);
                return;
            }
            var ccPayment = payment as CreditCardPayment;
            if (ccPayment != null)
            {
                CreditCardPayments.Add(ccPayment);
                return;
            }
            var compAmount = payment as CompAmountPayment;
            if (compAmount != null)
            {
                CompAmountPayments.Add(compAmount);
                return;
            }
            var compItem = payment as CompItemPayment;
            if (compItem != null)
            {
                CompItemsPayments.Add(compItem);
                return;
            }

            throw new ArgumentException("Can't process payment of type " + payment.GetType());
        }

        public virtual bool RemovePayment(IPayment payment)
        {
            //switch on the type to put in the right list 
            var cashPayment = payment as CashPayment;
            if (cashPayment != null)
            {
                return CashPayments.Remove(cashPayment);
            }
            var ccPayment = payment as CreditCardPayment;
            if (ccPayment != null)
            {
                return CreditCardPayments.Remove(ccPayment);
            }
            var compAmount = payment as CompAmountPayment;
            if (compAmount != null)
            {
                return CompAmountPayments.Remove(compAmount);
            }
            var compItem = payment as CompItemPayment;
            if (compItem != null)
            {
                return CompItemsPayments.Remove(compItem);
            }

            throw new ArgumentException("Can't process payment of type " + payment.GetType());
        }
        #endregion

        public List<DiscountAmount> DiscountAmounts;
        public List<DiscountPercentage> DiscountPercentages;
        public List<DiscountPercentageAfterMinimumCashTotal> DiscountPercentageAfterMinimumCashTotals;

        public virtual IEnumerable<IDiscount> GetDiscounts()
        {
            var list = new List<IDiscount>();
            list.AddRange(DiscountAmounts);
            list.AddRange(DiscountPercentages);
            list.AddRange(DiscountPercentageAfterMinimumCashTotals);

            return list;
        }

        public virtual void AddDiscount(IDiscount discount)
        {
            var amt = discount as DiscountAmount;
            if (amt != null)
            {
                DiscountAmounts.Add(amt);
                return;
            }

            var perc = discount as DiscountPercentage;
            if (perc != null)
            {
                DiscountPercentages.Add(perc);
                return;
            }

            var percAfterAmt = discount as DiscountPercentageAfterMinimumCashTotal;
            if (percAfterAmt != null)
            {
                DiscountPercentageAfterMinimumCashTotals.Add(percAfterAmt);
                return;
            }

            throw new ArgumentException("Can't process discount of type " + discount.GetType());
        }

        public bool RemoveDiscount(IDiscount discount)
        {
            var amt = discount as DiscountAmount;
            if (amt != null)
            {
                return DiscountAmounts.Remove(amt);
            }

            var perc = discount as DiscountPercentage;
            if (perc != null)
            {
                return DiscountPercentages.Remove(perc);
            }

            var percAfterAmt = discount as DiscountPercentageAfterMinimumCashTotal;
            if (percAfterAmt != null)
            {
                return DiscountPercentageAfterMinimumCashTotals.Remove(percAfterAmt);
            }

            throw new ArgumentException("Can't process discount of type " + discount.GetType());
        }


        #region Event processing

        /// <summary>
        /// Basic entry point for having an event processed by our entity
        /// </summary>
        /// <param name="checkEvent"></param>
        /// <returns></returns>
        public void When(ICheckEvent checkEvent)
        {
            //common fields
            LastTouchedServerID = checkEvent.EmployeeID;
            //when this event was created was the last time we updated the check
            LastUpdatedDate = checkEvent.CreatedDate;
            Revision = checkEvent.EventOrder;

            ProcessSpecificEvent(checkEvent);
        }

        /// <summary>
        /// Overrideable method to determine which events we support in this entity
        /// </summary>
        /// <param name="checkEvent"></param>
        /// <returns></returns>
        protected virtual void ProcessSpecificEvent(ICheckEvent checkEvent)
        {
            //event specific code for this
            switch (checkEvent.EventType)
            {
                case MiseEventTypes.CheckCreated:
                    WhenCheckCreated((CheckCreatedEvent)checkEvent);
                    break;
                case MiseEventTypes.CheckCreatedWithCreditCard:
                    WhenCheckCreatedWithCreditCard((CheckCreatedWithCreditCardEvent) checkEvent);
                    break;
                case MiseEventTypes.CheckSent:
                    WhenCheckSent((CheckSentEvent)checkEvent);
                    break;
                case MiseEventTypes.CheckReopened:
                    WhenCheckReopened((CheckReopenedEvent)checkEvent);
                    break;
                case MiseEventTypes.OrderItemModified:
                    WhenCheckOrderItemModified((OrderItemModifiedEvent)checkEvent);
                    break;
                case MiseEventTypes.MarkCheckAsPaid:
                    WhenCheckMarkAsPaid((MarkCheckAsPaidEvent)checkEvent);
                    break;
                case MiseEventTypes.ItemCompedGeneral:
                    WhenItemCompedGeneral((ItemCompedGeneralEvent)checkEvent);
                    break;
				case MiseEventTypes.ItemUncomped:
					WhenOrderItemCompRemoved ((ItemUncompedEvent)checkEvent);
					break;
                case MiseEventTypes.OrderOnCheck:
                    WhenOrderedOnCheck((OrderedOnCheckEvent)checkEvent);
                    break;
                case MiseEventTypes.OrderItemDeleted:
                    WhenOrderItemDeleted((OrderItemDeletedEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardChargeCompleted:
                    WhenCreditCardChargeCompleted((CreditCardChargeCompletedEvent)checkEvent);
                    break;
                case MiseEventTypes.OrderItemSetMemo:
                    WhenOrderItemSetMemo((OrderItemSetMemoEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardAddedForPayment:
                    WhenCreditCardAddedForPayment((CreditCardAddedForPaymentEvent)checkEvent);
                    break;
                case MiseEventTypes.CompPaidDirectlyOnCheck:
                    WhenCompPaidOnCheck((CompPaidDirectlyOnCheckEvent)checkEvent);
                    break;
                case MiseEventTypes.DiscountAppliedToCheck:
                    WhenDiscountAppliedToCheck((DiscountAppliedToCheckEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardFailedAuthorization:
                    WhenCreditCardFailedAuthorization((CreditCardFailedAuthorizationEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardCancelledEvent:
                    WhenCreditCardAuthorizationCancelled((CreditCardAuthorizationCancelledEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardAuthorizationStarted:
                    WhenCreditCardAuthorizationStarted((CreditCardAuthorizationStartedEvent)checkEvent);
                    break;
                case MiseEventTypes.CustomerAssignedToCheck:
                    WhenCustomerAssigned((CustomerAssignedToCheckEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardAuthorized:
                    WhenCreditCardAuthorized((CreditCardAuthorizedEvent)checkEvent);
                    break;
                case MiseEventTypes.CreditCardCloseRequested:
                    WhenCreditCardCloseRequested((CreditCardCloseRequestedEvent)checkEvent);
                    break;
                case MiseEventTypes.DiscountRemovedFromCheck:
                    WhenDiscountRemovedFromCheck((DiscountRemovedFromCheckEvent)checkEvent);
                    break;
                case MiseEventTypes.CredtCardTipAddedToCharge:
                    WhenCreditCardTipAddedToCharge((CreditCardTipAddedToChargeEvent)checkEvent);
                    break;
                case MiseEventTypes.CashPaidOnCheck:
                    WhenCashPaidOnCheck((CashPaidOnCheckEvent)checkEvent);
                    break;
                case MiseEventTypes.OrderItemVoided:
                    WhenOrderItemVoided((OrderItemVoidedEvent)checkEvent);
                    break;
				case MiseEventTypes.OrderItemWasted:
					WhenOrderItemWasted ((OrderItemWastedEvent)checkEvent);
					break;
                default:
                    throw new ArgumentException("Cannot apply event of type " + checkEvent.EventType + " to check type " + GetType());
            }
        }

        #region CreditCard events

        protected virtual void WhenCreditCardTipAddedToCharge(CreditCardTipAddedToChargeEvent add)
        {
            var payment = GetPayments().FirstOrDefault(p => p.Id == add.PaymentID) as ICreditCardPayment;
            if (payment == null)
            {
                throw new ArgumentException("Cannot find payment " + add.PaymentID + " to add tip to");
            }

            //add the tip
            payment.TipAmount = add.TipAmount;
            //update the status
            payment.PaymentProcessingStatus = PaymentProcessingStatus.WaitingToClose;

        }

        /// <summary>
        /// Mark that we've finished adding a tip, but not yet Z'ed
        /// </summary>
        /// <param name="closeReq"></param>
        protected virtual void WhenCreditCardCloseRequested(CreditCardCloseRequestedEvent closeReq)
        {
            PaymentStatus = CheckPaymentStatus.PaymentPending;

            var payment = GetPayments().FirstOrDefault(p => p.Id == closeReq.PaymentID) as ICreditCardPayment;
            if (payment == null)
            {
                throw new ArgumentException("Cannot find payment " + closeReq.PaymentID + " for CreditCardClose requested");
            }

            payment.PaymentProcessingStatus = PaymentProcessingStatus.WaitingToClose;
        }

        protected virtual void WhenCreditCardAuthorized(CreditCardAuthorizedEvent auth)
        {
            var ccPayments = GetPayments().OfType<ICreditCardPayment>();
            //get the payment
            var payment = ccPayments.FirstOrDefault(p => p.Id == auth.PaymentID);
            if (payment != null)
            {
                //update the payment
                payment.PaymentProcessingStatus = PaymentProcessingStatus.BaseAuthorized;
                payment.AuthorizationResult = auth.AuthorizationCode;
            }
            else
            {
                //error!
                throw new ArgumentException("Cannot find payment with ID " + auth.PaymentID);
            }
            UpdatePaymentStatusFromPayments();
        }

        protected virtual void WhenCreditCardAuthorizationStarted(CreditCardAuthorizationStartedEvent start)
        {
            var payment = GetPayments().FirstOrDefault(p => p.Id == start.PaymentID);
            if (payment == null)
            {
                throw new ArgumentException("Cannot find payment of ID " + start.PaymentID + " for CreditCardAuthorizationStartedEvent");
            }
            var ccPayment = payment as IProcessingPayment;
            if (ccPayment == null)
            {
                throw new ArgumentException("Payment " + start.PaymentID + " is not a processing payment, cannot start");
            }

            ccPayment.PaymentProcessingStatus = PaymentProcessingStatus.SentForBaseAuthorization;

            PaymentStatus = CheckPaymentStatus.PaymentPending;
        }

        protected virtual void WhenCreditCardAuthorizationCancelled(CreditCardAuthorizationCancelledEvent cancel)
        {
            var payment = GetPayments().FirstOrDefault(p => p.Id == cancel.PaymentID) as ICreditCardPayment;
            if (payment == null)
            {
                throw new ArgumentException("Cannot find payment with ID " + cancel.PaymentID);
            }

            payment.PaymentProcessingStatus = PaymentProcessingStatus.Cancelled;
            RemovePayment(payment);

            PaymentStatus = CheckPaymentStatus.Closing;
        }

        protected virtual void WhenCreditCardFailedAuthorization(CreditCardFailedAuthorizationEvent failed)
        {
            var ccPayments = GetPayments().OfType<ICreditCardPayment>();
            //get the payment
            var payment = ccPayments.FirstOrDefault(p => p.Id == failed.PaymentID);
            if (payment != null)
            {
                //update the payment
                payment.PaymentProcessingStatus = PaymentProcessingStatus.BaseRejected;
            }

            //check if we closed the last outstanding credit card payment.  If so change check status!
            UpdatePaymentStatusFromPayments();
        }

        protected virtual void WhenCreditCardAddedForPayment(CreditCardAddedForPaymentEvent paymentEvent)
        {
            var ccPayment = new CreditCardPayment
            {
                AmountCharged = paymentEvent.Amount,
                PaymentProcessingStatus = PaymentProcessingStatus.Created,
                Card = paymentEvent.CreditCard,
                CheckID = paymentEvent.CheckID,
                EmployeeID = paymentEvent.EmployeeID,
            };
            AddPayment(ccPayment);
            AddCreditCard(paymentEvent.CreditCard);
        }

        protected virtual void WhenCreditCardChargeCompleted(CreditCardChargeCompletedEvent complete)
        {
            var payment = GetPayments().FirstOrDefault(p => p.Id == complete.PaymentID) as ICreditCardPayment;
            if (payment == null)
            {
                throw new ArgumentException("No payment found for CreditCardChargeCompletedEvent!");
            }

            if (complete.WasAuthorized)
            {
                payment.PaymentProcessingStatus = PaymentProcessingStatus.Complete;
                PaymentStatus = CheckPaymentStatus.Closed;
            }
            else
            {
                payment.PaymentProcessingStatus = PaymentProcessingStatus.FullAmountRejected;
                PaymentStatus = CheckPaymentStatus.PaymentRejected;
            }
        }
        #endregion

        #region CompEvents

        protected virtual void WhenCompPaidOnCheck(CompPaidDirectlyOnCheckEvent comp)
        {
            			//make a payment
			var payment = new CompAmountPayment {
				AmountComped = comp.Amount,
				CheckID = comp.CheckID,
				EmployeeID = comp.EmployeeID
			};
            AddPayment(payment);
        }

        protected virtual void WhenItemCompedGeneral(ItemCompedGeneralEvent compGeneral)
        {
            var orderItem = OrderItems.FirstOrDefault(oi => oi.Id == compGeneral.OrderItemID);
            if (orderItem == null)
            {
                throw new ArgumentException("OrderItem not found for ItemCompedGeneralEvent");
            }

            orderItem.EmployeeWhoComped = compGeneral.EmployeeID;

            //we make a payment, but make sure it doesn't apply since our total will already exclude it
            var payment = new CompItemPayment
            {
                Amount = orderItem.Total,
                CheckID = Id,
                CreatedDate = compGeneral.CreatedDate,
                EmployeeID = compGeneral.EmployeeID,
                Reason = compGeneral.Reason,
				OrderItemID = compGeneral.OrderItemID
            };
            AddPayment(payment);
        }

		protected virtual void WhenOrderItemCompRemoved(ItemUncompedEvent uncomp){
			var orderItem = OrderItems.FirstOrDefault(oi => oi.Id == uncomp.OrderItemID);
			if (orderItem == null)
			{
				throw new ArgumentException("OrderItem not found for ItemCompedGeneralEvent");
			}

			orderItem.EmployeeWhoComped = null;

			//find the payment this is attached to
			var payment = CompItemsPayments.FirstOrDefault (p => p.OrderItemID == uncomp.OrderItemID);
			if(payment == null){
				throw new Exception ("Error, comp payment for order item not found on check");
			}
			CompItemsPayments.Remove (payment);
		}

        #endregion

        #region Discounts

        protected virtual void WhenDiscountRemovedFromCheck(DiscountRemovedFromCheckEvent remove)
        {
            var discount = GetDiscounts().FirstOrDefault(d => d.Id == remove.DiscountID);
            if (discount != null)
            {
                RemoveDiscount(discount);
            }
            throw new ArgumentException("Cannot find discount for removal");
        }
        protected virtual void WhenDiscountAppliedToCheck(DiscountAppliedToCheckEvent discount)
        {
            //don't let us double add a discount
            if (GetDiscounts().Contains(discount.Discount)) return;

            //let the discount check if it's valid!
            if (discount.Discount.CanApplyToCheck(this))
            {
                AddDiscount(discount.Discount);
            }
        }

        #endregion
		protected virtual void WhenOrderItemWasted(OrderItemWastedEvent wastedEvent){
			var foundItems = OrderItems.Where(oi => oi.Id == wastedEvent.OrderItemToVoid.Id).ToList();
			foreach (var oi in foundItems)
			{
				RemoveOrderItem(oi);
			}
		}
        protected virtual void WhenOrderItemVoided(OrderItemVoidedEvent voidedEvent)
        {
            var foundItems = OrderItems.Where(oi => oi.Id == voidedEvent.OrderItemToVoid.Id).ToList();
            foreach (var oi in foundItems)
            {
                RemoveOrderItem(oi);
            }
        }

        protected virtual void WhenCashPaidOnCheck(CashPaidOnCheckEvent cash)
        {
            var payment = new CashPayment
            {
                AmountPaid = cash.AmountPaid,
                AmountTendered = cash.AmountTendered,
                ChangeGiven = cash.ChangeGiven
            };

            AddPayment(payment);
            ChangeDue = ChangeDue.Add(cash.ChangeGiven);
        }

        protected virtual void WhenCustomerAssigned(CustomerAssignedToCheckEvent cust)
        {
            Customer = cust.Customer;
        }

        protected virtual void WhenOrderItemSetMemo(OrderItemSetMemoEvent memo)
        {
            var orderItem = OrderItems.FirstOrDefault(oi => oi.Id == memo.OrderItemID);
            if (orderItem != null)
            {
                orderItem.Memo = memo.Memo;
            }
            else
            {
                throw new ArgumentException("OrderItem not found for OrderItemSetMemoEvent");
            }
        }

        protected virtual void WhenOrderedOnCheck(OrderedOnCheckEvent ordered)
        {
            AddOrderItem(ordered.OrderItem);
        }


        protected virtual void WhenCheckMarkAsPaid(MarkCheckAsPaidEvent markPaid)
        {
            if (Total.HasValue == false)
            {
                ClosedDate = DateTime.UtcNow;
                PaymentStatus = CheckPaymentStatus.Closed;
            }
            else
            {
                UpdatePaymentStatusFromPayments();
            }
        }

        protected virtual void WhenCheckCreated(CheckCreatedEvent created)
        {
            CreatedDate = created.CreatedDate;
            Id = created.CheckID;
            RestaurantID = created.RestaurantId;
            CreatedByServerID = created.EmployeeID;
            LastTouchedServerID = created.EmployeeID;
        }

        protected virtual void WhenCheckCreatedWithCreditCard(CheckCreatedWithCreditCardEvent created)
        {
            WhenCheckCreated(created);
            AddCreditCard(created.CreditCard);
        }

        protected virtual void WhenCheckSent(CheckSentEvent sent)
        {
            //mark all order items in the check as sent
            var unsaved = OrderItems.Where(oi => oi.Status == OrderItemStatus.Added);
            foreach (var oi in unsaved)
            {
                oi.Status = OrderItemStatus.Sent;
            }
        }

        protected virtual void WhenCheckReopened(CheckReopenedEvent reopened)
        {
            PaymentStatus = CheckPaymentStatus.Open;
        }

        protected virtual void WhenCheckOrderItemModified(OrderItemModifiedEvent mod)
        {
            //get the OI
            var orderItem = OrderItems.FirstOrDefault(oi => oi.Id == mod.OrderItemID);
            if (orderItem == null)
            {
                return;
            }


            orderItem.ClearModifiers();
            foreach (var m in mod.Modifiers)
            {
                orderItem.AddModifier(m);
            }
        }

        protected virtual void WhenOrderItemDeleted(OrderItemDeletedEvent deletedEvent)
        {
            //get the OI
            var item = OrderItems.FirstOrDefault(oi => oi.Id == deletedEvent.OrderItemID);
            if (item == null)
            {
                throw new ArgumentException("No OrderItem exists to delete!");
            }

            _orderItems.Remove(item);
        }
        #endregion

    }
}
