using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems;
using Mise.Core.ValueItems.Inventory;
using NUnit.Framework;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities;
namespace Mise.Core.Common.UnitTests.Entities.Inventory
{
    [TestFixture]
    public class ReceivingOrderTests
    {
        [Test]
        public void CloneShouldCloneContainerAndQuantityAndPriceInLineItems()
        {
            var ro = new ReceivingOrder
            {
                Id = Guid.NewGuid(),
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1011},
                LineItems = new List<ReceivingOrderLineItem>
                {
                    new ReceivingOrderLineItem
                    {
                        Id = Guid.NewGuid(),
                        MiseName = "testLI",
                        LineItemPrice = new Money(100.0M),
                        Quantity = 1012,
                        Container = new LiquidContainer
                        {
                            AmountContained = new LiquidAmount {Milliliters = 19, SpecificGravity = .08M},
                            WeightEmpty = new Weight {Grams = 101}
                        }
                    }
                }
            };

            //ACT
            var res = ro.Clone() as IReceivingOrder;

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(ro.Id, res.Id);

            var roLi = ro.GetBeverageLineItems().First();
            var resLi = res.GetBeverageLineItems().First();

            Assert.AreEqual(roLi.Quantity, resLi.Quantity);
            Assert.IsTrue(roLi.LineItemPrice.Equals(resLi.LineItemPrice), "Price");
            Assert.NotNull(resLi.Container);
            Assert.IsTrue(roLi.Container.Equals(roLi.Container), "Container");
            Assert.AreEqual(roLi.Container.AmountContained.Milliliters, resLi.Container.AmountContained.Milliliters, "ml");
            Assert.AreEqual(roLi.Container.AmountContained.SpecificGravity, resLi.Container.AmountContained.SpecificGravity, "sg");
            Assert.AreEqual(roLi.Container.WeightEmpty.Grams, resLi.Container.WeightEmpty.Grams);
        }

		[Test]
		public void CreateEventShouldCreateWithCorrectStatus(){
			var cEv = new ReceivingOrderCreatedEvent {
				ReceivingOrderID = Guid.NewGuid (),
				CausedById = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				EventOrder = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 12}
			};

			var underTest = new ReceivingOrder ();

			//ACT
			underTest.When (cEv);

			//ASSERT
			Assert.AreEqual(ReceivingOrderStatus.Created,underTest.Status, "currently processing");
			Assert.AreEqual (cEv.ReceivingOrderID, underTest.Id);
			Assert.AreEqual (cEv.CausedById, underTest.ReceivedByEmployeeID);
			Assert.AreEqual (cEv.CreatedDate, underTest.CreatedDate );
			Assert.AreEqual (cEv.CreatedDate, underTest.LastUpdatedDate);
			Assert.AreEqual (cEv.EventOrder.OrderingID, underTest.Revision.OrderingID);
		}

		[Test]
		public void CompleteEventShouldChangeStatusToComplete(){
			var underTest = new ReceivingOrder{
				Status = ReceivingOrderStatus.Created,
				ReceivedByEmployeeID = Guid.NewGuid ()
			};

			var ev = new ReceivingOrderCompletedEvent
			{
				CausedById = Guid.NewGuid (),
				CreatedDate = DateTimeOffset.UtcNow,
				RestaurantId = Guid.NewGuid (),
				EventOrder = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10102}
			};

			//ACT
			underTest.When(ev);

			//Assert
			Assert.AreEqual(ReceivingOrderStatus.Completed, underTest.Status);
			Assert.AreNotEqual (ev.CausedById, underTest.ReceivedByEmployeeID);
		}
    }
}
