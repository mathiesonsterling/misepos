using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Neo4J.Neo4JDAL;
using Mono.Security.Cryptography;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class AccountTests
    {
        private Neo4JEntityDAL _underTest;

        [SetUp]
        public void Setup()
        {
            var logger = new Mock<ILogger>();
            _underTest = new Neo4JEntityDAL(TestUtilities.GetConfig(), logger.Object);

            _underTest.ResetDatabase();
        }

        [Test]
        public async Task AccountCRUD()
        {
            var creditCard = new CreditCard
            {
                Name = PersonName.TestName,
                ProcessorToken = new CreditCardProcessorToken
                {
                    Processor = CreditCardProcessors.FakeProcessor,
                    Token = "fake"
                }
            };
            var creditCard2 = new CreditCard
            {
                Name = new PersonName("Billy", "Bob", "Jenkins"),
                ProcessorToken = null
            };

            var acctID = Guid.NewGuid();
            var account = new RestaurantAccount
            {
                ID = acctID,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 1},

                AccountHolderName = PersonName.TestName,
                AppsOnAccount = new List<MiseAppTypes> {MiseAppTypes.UnitTests, MiseAppTypes.POSMobile},
                BillingCycle = new TimeSpan(30, 0, 0, 0),
                CurrentCard = creditCard,
                PhoneNumber = PhoneNumber.TestPhoneNumber,
                Status = MiseAccountStatus.Active,
                PrimaryEmail = EmailAddress.TestEmail,
                Emails = new List<EmailAddress> {EmailAddress.TestEmail},
                ReferralCodeUsedToCreate = new ReferralCode("CREATED_CODE"),
                ReferralCodeForAccountToGiveOut = new ReferralCode("GIVE_CODE"),
                Charges = new List<AccountCharge>
                {
                    new AccountCharge
                    {
                        ID = Guid.NewGuid(),
                        AccountID = acctID,
                        Amount = new Money(10.0M),
                        App = MiseAppTypes.UnitTests,
                        CreatedDate = DateTime.UtcNow,
                        DateEnd = DateTime.UtcNow.AddDays(-1),
                        DateStart = DateTime.UtcNow.AddDays(-100),
                        LastUpdatedDate = DateTime.UtcNow,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 2}
                    },
                    new AccountCharge
                    {
                        ID = Guid.NewGuid(),
                        AccountID = acctID,
                        Amount = new Money(9.12M),
                        App = MiseAppTypes.POSMobile,
                        CreatedDate = DateTime.UtcNow,
                        DateStart = DateTime.UtcNow.AddMonths(-1),
                        DateEnd = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 3}
                    }
                },
                Payments = new List<AccountCreditCardPayment>
                {
                    new AccountCreditCardPayment
                    {
                        ID = Guid.NewGuid(),
                        AccountID = acctID,
                        Amount = new Money(9.99M),
                        CardUsed = creditCard,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 12},
                        Status = PaymentProcessingStatus.Empty
                    },
                     new AccountCreditCardPayment
                     {
                        ID = Guid.NewGuid(),
                        AccountID = acctID,
                        Amount = new Money(9.99M),
                        CardUsed = creditCard2,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 12},
                        Status = PaymentProcessingStatus.Empty
                    }
                },
                AccountCredits = new List<AccountCredit>
                {
                    new AccountCredit
                    {
                        ID = Guid.NewGuid(),
                        AccountID = acctID,
                        Amount = new Money(100),
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        Revision = new EventID{AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 5},
                        ReferralCodeGiven = new ReferralCode("TESTREFCODE"),
                        Status = PaymentProcessingStatus.CreditNeedsProcessing,
                    }
                }
            };

            await _underTest.AddAccountAsync(account);

            var gots = (await _underTest.GetAccountsAsync()).ToList();

            Assert.NotNull(gots);
            var retAcct = gots.First();
            Assert.AreEqual(account.ID, retAcct.ID);
            Assert.AreEqual(account.CurrentCard, retAcct.CurrentCard, "current credit card");

            Assert.NotNull(retAcct.ReferralCodeUsedToCreate);
            Assert.AreEqual("CREATED_CODE", retAcct.ReferralCodeUsedToCreate.Code);
            Assert.NotNull(retAcct.ReferralCodeForAccountToGiveOut);
            Assert.AreEqual("GIVE_CODE", retAcct.ReferralCodeForAccountToGiveOut.Code);
            var retCharges = retAcct.GetCharges().ToList();
            Assert.AreEqual(account.Charges.Count, retCharges.Count());
            foreach (var c in retCharges)
            {
                var matchedSource = account.GetCharges().FirstOrDefault(oc => oc.ID == c.ID);
                Assert.NotNull(matchedSource);
                Assert.AreEqual(c.AccountID, matchedSource.AccountID);
                Assert.AreEqual(c.Amount, matchedSource.Amount);
                Assert.AreEqual(c.App, matchedSource.App, "App");
                Assert.AreEqual(matchedSource.DateEnd, c.DateEnd);
                Assert.AreEqual(matchedSource.DateStart, c.DateStart);
            }

            var retCreditCardPayments =
                retAcct.GetPayments().Where(c => c is AccountCreditCardPayment).Cast<AccountCreditCardPayment>().ToList();
            Assert.AreEqual(account.Payments.Count, retCreditCardPayments.Count, "num cc payments");
            foreach (var pmt in retCreditCardPayments)
            {
                var matchedSource = account.GetPayments().FirstOrDefault(p => p.ID == pmt.ID) as AccountCreditCardPayment;
                Assert.NotNull(matchedSource);
                Assert.AreEqual(pmt.AccountID, matchedSource.AccountID, "accountID on payment");
                Assert.AreEqual(pmt.Amount, matchedSource.Amount);
                Assert.AreEqual(pmt.CardUsed, matchedSource.CardUsed, "credit card");
            }

            var retCredits = retAcct.GetPayments().Where(c => c is AccountCredit).Cast<AccountCredit>().ToList();
            Assert.AreEqual(account.AccountCredits.Count, retCredits.Count);
            foreach (var cred in retCredits)
            {
                var match = account.AccountCredits.FirstOrDefault(p => p.ID == cred.ID);
                Assert.NotNull(match);

                Assert.AreEqual(match.Amount, cred.Amount, "credit amount");
                Assert.AreEqual(match.ReferralCodeGiven, cred.ReferralCodeGiven);
            }

            //TODO - updated
        }
    }
}
