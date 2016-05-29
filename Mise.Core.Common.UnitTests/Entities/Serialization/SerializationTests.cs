using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.People;
using NUnit.Framework;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.People;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.ValueItems;


namespace Mise.Core.Common.UnitTests.Entities.Serialization
{
	[TestFixture]
	public class SerializationTests
	{
		IJSONSerializer _jsonSer;

		[SetUp]
		public void Setup(){
			_jsonSer = new JsonNetSerializer ();
		}

		/// <summary>
		/// Tests our BarTab serializes and desers
		/// </summary>
		[Test]
		public void TestCheckEmpty(){
			var checkID = Guid.NewGuid ();
			var barTab = new RestaurantCheck{
				Id = checkID,
				CreatedDate = DateTime.Now,
				Customer = new Customer {
					Id = Guid.NewGuid (),
					Name = PersonName.TestName,
					LastUpdatedDate = DateTime.Today,
					CreatedDate = DateTime.Today,
				},
				CreatedByServerID = Guid.NewGuid (),
				LastTouchedServerID = Guid.NewGuid (),
			};
				

			var json = _jsonSer.Serialize (barTab);

			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = _jsonSer.Deserialize<RestaurantCheck> (json);
			Assert.IsNotNull (res);
		}

        /// <summary>
        /// Tests our BarTab serializes and desers
        /// </summary>
        [Test]
        public void TestBarTabWithCashPayment()
        {
            var checkID = Guid.NewGuid();
            var barTab = new RestaurantCheck
            {
                Id = checkID,
                CreatedDate = DateTime.Now,
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = PersonName.TestName,
                    LastUpdatedDate = DateTime.Today,
                    CreatedDate = DateTime.Today,
                },
                CreatedByServerID = Guid.NewGuid(),
                LastTouchedServerID = Guid.NewGuid(),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem
                        {
                            Price=new Money(10.0M)
                        }
                    }
                },
            };
            barTab.AddPayment(new CashPayment
            {
                AmountPaid = new Money(10.0M)
            });


            var json = _jsonSer.Serialize(barTab);

            Assert.IsFalse(string.IsNullOrEmpty(json));

            var res = _jsonSer.Deserialize<RestaurantCheck>(json);
            Assert.IsNotNull(res);

            //check all payments and order items came through!
        }


        /// <summary>
        /// Tests our BarTab serializes and desers
        /// </summary>
        [Test]
        public void TestCheckWithCreditPayment()
        {
            var checkID = Guid.NewGuid();
            var barTab = new RestaurantCheck
            {
                Id = checkID,
                CreatedDate = DateTime.Now,
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = PersonName.TestName,
                    LastUpdatedDate = DateTime.Today,
                    CreatedDate = DateTime.Today,
                },
                CreatedByServerID = Guid.NewGuid(),
                LastTouchedServerID = Guid.NewGuid(),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem
                        {
                            Price=new Money(10.0M)
                        }
                    }
                },
            };
            barTab.AddPayment(new CreditCardPayment
            {
                AmountCharged = new Money(10.0M),
                AuthorizationResult = new CreditCardAuthorizationCode
                {
                    IsAuthorized = true
                }
            });


            var json = _jsonSer.Serialize(barTab);

            Assert.IsFalse(string.IsNullOrEmpty(json));

            var res = _jsonSer.Deserialize<RestaurantCheck>(json);
            Assert.IsNotNull(res);

            //check all payments and order items came through!
        }


        /// <summary>
        /// Tests our BarTab serializes and desers
        /// </summary>
        [Test]
        public void TestCheckWithPercentDiscount()
        {
            var checkID = Guid.NewGuid();
            var barTab = new RestaurantCheck
            {
                Id = checkID,
                CreatedDate = DateTime.Now,
                Customer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = PersonName.TestName,
                    LastUpdatedDate = DateTime.Today,
                    CreatedDate = DateTime.Today,
                },
                CreatedByServerID = Guid.NewGuid(),
                LastTouchedServerID = Guid.NewGuid(),
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        MenuItem = new MenuItem
                        {
                            Price=new Money(10.0M)
                        }
                    }
                },
            };

            barTab.AddDiscount(new DiscountPercentage
            {
                Percentage = .10M
            });


            var json = _jsonSer.Serialize(barTab);

            Assert.IsFalse(string.IsNullOrEmpty(json));

            var res = _jsonSer.Deserialize<RestaurantCheck>(json);
            Assert.IsNotNull(res);

            
            Assert.AreEqual(1, res.OrderItems.Count());
            Assert.AreEqual(1, res.GetDiscounts().Count());

            var firstDiscount = res.GetDiscounts().First() as DiscountPercentage;
            Assert.IsNotNull(firstDiscount);
            Assert.AreEqual(.10M, firstDiscount.Percentage);
        }

		[Test]
		public void TestCustomer()
		{
		    var date = DateTimeOffset.UtcNow;
			var customer = new Customer {
				Id = Guid.NewGuid (),
				Name = PersonName.TestName,
				LastUpdatedDate = date,
				CreatedDate =date,
			};

			var json = _jsonSer.Serialize (customer);

			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = _jsonSer.Deserialize<Customer> (json);
			Assert.IsNotNull (res);
			Assert.AreEqual (date, res.CreatedDate);
			Assert.AreEqual (date, res.LastUpdatedDate);
			Assert.AreEqual (PersonName.TestName, res.Name);
		}

		[Test]
		public void TestMoney(){
			var money = new Money (10.98M);

			var json = _jsonSer.Serialize (money);
			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = _jsonSer.Deserialize<Money> (json);

			Assert.IsNotNull (res);
			Assert.AreEqual (10.98M, res.Dollars);
		}

		[Test]
		public void TestOrderItem(){
			var oi = new OrderItem {
				Id = Guid.NewGuid (),
				MenuItem = new MenuItem {
					Id = Guid.NewGuid (),
					Price = new Money (1.05M),
					DisplayWeight = 10,
					Name = "testMI",
					ButtonName = "test",
					OIListName = "test"
				},
				Status = OrderItemStatus.Made
			};

			var json = _jsonSer.Serialize (oi);
			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = _jsonSer.Deserialize<OrderItem> (json);

			Assert.IsNotNull (res);
			Assert.AreEqual (OrderItemStatus.Made, res.Status);
			Assert.IsNotNull (res.MenuItem);
			Assert.AreEqual (1.05M, res.MenuItem.Price.Dollars);
		}

		[Test]
		public void TestEmployee(){
			var id = Guid.NewGuid ();
			var emp = new Employee {
				Id = id,
				Name = PersonName.TestName,
				//CompBudget = new Money (10.0M),
				//CurrentlyClockedInToPOS = true,
				Emails = new List<EmailAddress> {
					new EmailAddress{ Value = "test@test.com" }
				},
				//Passcode = "1234",
			};

			var json = _jsonSer.Serialize (emp);
			Assert.IsFalse (string.IsNullOrEmpty (json));

			var res = _jsonSer.Deserialize<Employee> (json);
			Assert.IsNotNull (res);
			Assert.AreEqual (id, emp.Id);
			Assert.AreEqual ("Testy", res.Name.FirstName);
			Assert.AreEqual ("McTesterson", res.Name.LastName);
			//Assert.AreEqual (10.0M, res.CompBudget.Dollars);

			Assert.IsNotNull (res.Emails);
			Assert.AreEqual ("test@test.com", res.Emails.First().Value);
		}
	}
}

