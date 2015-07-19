using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Accounts;
using Mise.Core.Common.Services.Implementation;
using Mise.Core.Entities;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Neo4J;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    //ignored for now jsut for how long they run (4M)
    [Ignore]
    [TestFixture]
    public class MenuTests
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
        public async Task MenuAndAccountShouldCreateWithoutException()
        {
            await RunIt();
        }

        private async Task RunIt()
        {
            var service = new FakeDomineesRestaurantServiceClient();
            var menu = await service.GetMenusAsync();
            var restaurant = (await service.RegisterClientAsync("")).Item1;
            var account = new RestaurantAccount
            {
                ID = restaurant.AccountID.HasValue ? restaurant.AccountID.Value : Guid.Empty,
                CreatedDate = DateTime.UtcNow.AddMonths(-2),
                Emails = new List<EmailAddress> { new EmailAddress { Value = "mathieson@misepos.com" } },
                LastUpdatedDate = DateTime.UtcNow,
                PhoneNumber = new PhoneNumber { AreaCode = "718", Number = "7152945" },
                ReferralCodeForAccountToGiveOut = new ReferralCode("mattyboi"),
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 15 }
            };

            await _underTest.AddAccountAsync(account);

            await _underTest.AddRestaurantAsync(restaurant);
            var empsFromService = service.GetEmployees().ToList();
            foreach (var emp in empsFromService)
            {
                await _underTest.AddEmployeeAsync(emp);
            }

            var fMenu = menu.FirstOrDefault();
            await _underTest.AddMenuAsync(fMenu);


            Assert.IsTrue(true, "GraphDatabase Created");
            var empsFromDB = await _underTest.GetEmployeesAsync(restaurant.ID);
            Assert.AreEqual(empsFromService.Count(), empsFromDB.Count(), "Employee count");

            //get the account
            var accounts = await _underTest.GetAccountsAsync();

            Assert.NotNull(accounts);
            Assert.AreEqual(1, accounts.Count(), "has one account");
        }
    }
}
