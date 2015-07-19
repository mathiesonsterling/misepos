using System.Collections.Generic;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using NUnit.Framework;
using Mise.Core.Entities.Check;
using Mise.Core.Common.Entities;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events.Payments;
namespace Mise.Core.Common.UnitTests.Entities.Events
{
	[TestFixture]
	public class MarkCheckAsPaidTests
    {

        [Test]
		public void TestMarkingCheckAsPaidClosesEmptyCheck(){
			var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};
			
			var check = new RestaurantCheck{PaymentStatus = CheckPaymentStatus.Closing};

			//ACT
            check.When(checkEvent);

			//ASSERT
			Assert.AreEqual (CheckPaymentStatus.Closed, check.PaymentStatus, "check is closed");
		}

        [Test]
        public void TestMarkingCheckAsPaidDoesntCloseUnPaidCheck()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };

            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closing, check.PaymentStatus, "check is closed");
        }

        #region Cash
        [Test]
        public void TestMarkingCheckAsPaidClosesPaidCheckExactAmountCash()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CashPayment { AmountPaid = new Money(100.0M) });
            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closed, check.PaymentStatus, "check is closed");
        }

        [Test]
        public void TestMarkingCheckAsPaidClosesPaidCheckOverAmountCash()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CashPayment
            {
                AmountPaid = new Money(10.0M)
            });
            check.AddPayment(new CashPayment{AmountPaid = new Money(110.0M)});
            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closed, check.PaymentStatus, "check is closed");
        }


        [Test]
        public void TestMarkingCheckAsPaidDoesntClosePartiallyPaidCheckCash()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CashPayment
            {
                AmountPaid = new Money(10.0M)
            });
            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closing, check.PaymentStatus, "check is closed");
        }

        #endregion

        #region Credit
        [Test]
        public void TestMarkingCheckAsPaidClosesPaidCheckExactAmountCredit()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CreditCardPayment { AmountCharged = new Money(100.0M), PaymentProcessingStatus = PaymentProcessingStatus.Complete });
            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closed, check.PaymentStatus, "check is closed");
        }

		[Test]
		public void TestMarkingCheckAsPaidRejectedPaymentCantClose()
		{
			var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

			var check = new RestaurantCheck
			{
				PaymentStatus = CheckPaymentStatus.PaymentPending,
				OrderItems = new List<OrderItem >
				{
					new OrderItem
					{
						MenuItem = new MenuItem{Price = new Money(100.0M)}
					}
				}
			};
			check.AddPayment(new CreditCardPayment { AmountCharged = new Money(100.0M), PaymentProcessingStatus = PaymentProcessingStatus.BaseRejected});
			//ACT
            check.When(checkEvent);

			//ASSERT
			Assert.AreEqual(CheckPaymentStatus.PaymentRejected, check.PaymentStatus, "status is payment rejected");
		}

		[Test]
		public void TestMarkingCheckAsPaidRejectedPaymentWithAValidOneCanClose()
		{
			var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

			var check = new RestaurantCheck
			{
				PaymentStatus = CheckPaymentStatus.PaymentPending,
				OrderItems = new List<OrderItem >
				{
					new OrderItem
					{
						MenuItem = new MenuItem{Price = new Money(100.0M)}
					}
				}
			};
			check.AddPayment(new CreditCardPayment { AmountCharged = new Money(100.0M), PaymentProcessingStatus = PaymentProcessingStatus.BaseRejected});
			check.AddPayment(new CreditCardPayment { AmountCharged = new Money(100.0M), PaymentProcessingStatus = PaymentProcessingStatus.Complete});
			//ACT
            check.When(checkEvent);

			//ASSERT
			Assert.AreEqual(CheckPaymentStatus.Closed, check.PaymentStatus, "status is closed");
			Assert.AreEqual (Money.None, check.ChangeDue);
		}

        [Test]
        public void TestMarkingCheckAsPaidClosesPaidCheckOverAmountCredit()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CashPayment
            {
                AmountPaid = new Money(10.0M)
            });
            check.AddPayment(new CreditCardPayment { AmountCharged = new Money(110.0M), PaymentProcessingStatus = PaymentProcessingStatus.Complete });
            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closed, check.PaymentStatus, "check is closed");
        }


        [Test]
        public void TestMarkingCheckAsPaidDoesntClosePartiallyPaidCheckCredit()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CreditCardPayment
            {
                AmountCharged = new Money(10.0M),
                PaymentProcessingStatus = PaymentProcessingStatus.Complete 
            });
            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closing, check.PaymentStatus, "check is closed");
        }
        #endregion

        #region Mixing it up
        [Test]
        public void TestMarkingCheckAsPaidClosesPaidCheckOverAmountCashAndCreditPending()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CashPayment
            {
                AmountPaid = new Money(10.0M)
            });
            check.AddPayment(new CreditCardPayment {AmountCharged = new Money(100.0M), PaymentProcessingStatus = PaymentProcessingStatus.SentForBaseAuthorization});

            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.PaymentPending, check.PaymentStatus, "check is pending");
        }

        [Test]
        public void TestMarkingCheckAsPaidClosesPaidCheckOverAmountCashAndCreditClosed()
        {
            var checkEvent = new MarkCheckAsPaidEvent{EventOrderingID = new EventID()};

            var check = new RestaurantCheck
            {
                PaymentStatus = CheckPaymentStatus.Closing,
                OrderItems = new List<OrderItem >
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem{Price = new Money(100.0M)}
                    }
                }
            };
            check.AddPayment(new CashPayment
            {
                AmountPaid = new Money(10.0M)
            });
            check.AddPayment(new CreditCardPayment { AmountCharged = new Money(100.0M), PaymentProcessingStatus = PaymentProcessingStatus.Complete});

            //ACT
            check.When(checkEvent);

            //ASSERT
            Assert.AreEqual(CheckPaymentStatus.Closed, check.PaymentStatus, "check is closed");
        }
        #endregion 
    }
}

