using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities;
using Mise.Core.ValueItems;
using NUnit.Framework;

namespace Mise.Core.Common.UnitTests.Entities
{
    [TestFixture]
    public class RestaurantAccountTests
    {
        [Test]
        public void CloneShould()
        {
            var account = new RestaurantAccount
            {
                ID = Guid.NewGuid(),
                AccountHolderName = PersonName.TestName,
                BillingCycle = new TimeSpan(30, 0, 0, 0),
                Charges = new List<AccountCharge>
                {
                    new AccountCharge
                    {
                        ID = Guid.NewGuid(),
                        Amount = new Money(10.0M),
                        App = MiseAppTypes.POSMobile
                    }
                },
                CreatedDate = DateTime.UtcNow,
                CurrentCard = new CreditCard
                {
                    Name = PersonName.TestName,
                },
                PrimaryEmail = EmailAddress.TestEmail,
                Emails = new List<EmailAddress> {EmailAddress.TestEmail},
                LastUpdatedDate = DateTime.UtcNow,
                Payments = new List<AccountCreditCardPayment>
                {
                    new AccountCreditCardPayment
                    {
                        ID = Guid.NewGuid(),
                        Amount = new Money(11.19M)
                    }
                },
                Status = MiseAccountStatus.FreeDemoPeriod,
                PhoneNumber = PhoneNumber.TestPhoneNumber,
                ReferralCodeForAccountToGiveOut = ReferralCode.TestReferralCode,
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 10}
            };

            //ACT
            var res = account.Clone() as RestaurantAccount;

            //ASSERT
            Assert.NotNull(res);
            Assert.AreEqual(account.ID, res.ID);
            Assert.AreEqual(account.AccountHolderName, res.AccountHolderName);
            Assert.AreEqual(account.BillingCycle, res.BillingCycle);
            Assert.AreEqual(account.Charges.Count(), res.Charges.Count(), "num charges");
            Assert.AreEqual(account.Charges.First().ID, res.Charges.First().ID, "charge ID");
            Assert.AreEqual(account.Charges.First().App, res.Charges.First().App);
            Assert.AreEqual(account.Charges.First().Amount, res.Charges.First().Amount);
            Assert.AreEqual(account.CreatedDate, res.CreatedDate, "Created Date");

            Assert.AreEqual(account.Status, res.Status, "Status");
        }

        [Test]
        public void CreateAccountShouldGiveTimeSpanAndID()
        {
            
        }

        [Test]
        public void AccountCreditsShouldThrowIfImproperPaymentStatusGiven()
        {
            var underTest = new AccountCredit
            {
                ID = Guid.NewGuid(),
                Status = PaymentProcessingStatus.CreditNeedsProcessing
            };

            //test good
            underTest.Status = PaymentProcessingStatus.CreditProcessed;

            Assert.AreEqual(PaymentProcessingStatus.CreditProcessed, underTest.Status);

            var threw = false;
            try
            {
                underTest.Status = PaymentProcessingStatus.FullAmountRejected;
            }
            catch (ArgumentException)
            {
                threw = true;
            }

            Assert.IsTrue(threw);
        }
    }
}
