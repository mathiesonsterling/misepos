using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Neo4J.Neo4JDAL;
using Moq;
using NUnit.Framework;

namespace Mise.Database.Neo4J.IntegrationTests
{
    [TestFixture]
    public class ApplicationInvitationTests
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
        public async Task InvitationCrudWithoutDestinationEmployee()
        {
            var rest = TestUtilities.CreateRestaurant();
            rest.Name = new RestaurantName("Test Rest");
            await _underTest.AddRestaurantAsync(rest);

            var invitingEmp = TestUtilities.CreateEmployee();
            invitingEmp.Emails = new List<EmailAddress>{new EmailAddress{Value = "testInvitingEmp@test.com"}};
            invitingEmp.Name = PersonName.TestName;
            invitingEmp.RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>
            {
                {rest.ID, new[]{MiseAppTypes.UnitTests}}
            };
            await _underTest.AddEmployeeAsync(invitingEmp);

            var destEmp = TestUtilities.CreateEmployee();
            destEmp.Emails = new List<EmailAddress>{new EmailAddress{Value = "updated@mise.com"}};
            destEmp.Name = new PersonName("upper", "test");
            await _underTest.AddEmployeeAsync(destEmp);


            var destEmail = new EmailAddress {Value = "destEmail@another.com"};

            var appInvite = new ApplicationInvitation
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Application = MiseAppTypes.UnitTests,
                DestinationEmail = destEmail,
                DestinationEmployeeID = null,
                InvitingEmployeeID = invitingEmp.ID,
                InvitingEmployeeName = invitingEmp.Name,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                RestaurantName = rest.Name,
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100},
                Status = InvitationStatus.Created
            };

            await _underTest.AddApplicationInvitiation(appInvite);

            var getForEmail = (await _underTest.GetOpenApplicationInvitations(destEmail)).ToList();

            Assert.NotNull(getForEmail);
            Assert.AreEqual(1, getForEmail.Count());

            var item = getForEmail.First();
            Assert.AreEqual(appInvite.ID, item.ID, "ID");
            Assert.AreEqual(appInvite.Status, item.Status, "Status");
            Assert.AreEqual(appInvite.DestinationEmail, destEmail, "Dest email");
            Assert.AreEqual(appInvite.RestaurantID, item.RestaurantID, "Restauarnt ID");
            Assert.AreEqual(appInvite.RestaurantName, item.RestaurantName, "Restaurant Name");
            Assert.AreEqual(appInvite.InvitingEmployeeID, item.InvitingEmployeeID);
            Assert.AreEqual(appInvite.InvitingEmployeeName, item.InvitingEmployeeName);
            //UPDATE
            var updatedInvite = new ApplicationInvitation
            {
                ID = appInvite.ID,
                CreatedDate = DateTime.UtcNow,
                Application = MiseAppTypes.UnitTests,
                DestinationEmail = new EmailAddress{Value = "updated@mise.com"},
                DestinationEmployeeID = destEmp.ID,
                InvitingEmployeeID = invitingEmp.ID,
                InvitingEmployeeName = invitingEmp.Name,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                RestaurantName = rest.Name,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100 },
                Status = InvitationStatus.Rejected
            };

            await _underTest.UpdateApplicationInvitation(updatedInvite);

            //GET BY EMAIL
            var getForRest = await _underTest.GetApplicationInvitations();
            item = getForRest.First();
            Assert.AreEqual(updatedInvite.ID, item.ID, "ID");
            Assert.AreEqual(updatedInvite.Status, item.Status, "Status updated");
            Assert.NotNull(updatedInvite.DestinationEmail);
            Assert.AreEqual("updated@mise.com", updatedInvite.DestinationEmail.Value, "Dest email updated");
            Assert.AreEqual(updatedInvite.RestaurantID, item.RestaurantID, "Restauarnt ID");
            Assert.AreEqual(updatedInvite.RestaurantName, item.RestaurantName, "Restaurant Name");

            Assert.AreEqual(updatedInvite.DestinationEmployeeID, item.DestinationEmployeeID, "dest ID");
            Assert.AreEqual(updatedInvite.InvitingEmployeeID, item.InvitingEmployeeID);
            Assert.AreEqual(updatedInvite.InvitingEmployeeName, item.InvitingEmployeeName);
        }

        [Test]
        public async Task UpdateShouldOnlyAffectOneItem()
        {
            var rest = TestUtilities.CreateRestaurant();
            rest.Name = new RestaurantName("Test Rest");
            await _underTest.AddRestaurantAsync(rest);

            var invitingEmp = TestUtilities.CreateEmployee();
            invitingEmp.Emails = new List<EmailAddress> { new EmailAddress { Value = "testInvitingEmp@test.com" } };
            invitingEmp.Name = PersonName.TestName;
            invitingEmp.RestaurantsAndAppsAllowed = new Dictionary<Guid, IList<MiseAppTypes>>
            {
                {rest.ID, new[]{MiseAppTypes.UnitTests}}
            };
            await _underTest.AddEmployeeAsync(invitingEmp);

            var destEmp = TestUtilities.CreateEmployee();
            destEmp.Emails = new List<EmailAddress> { new EmailAddress { Value = "updated@mise.com" } };
            destEmp.Name = new PersonName("Upper", "Test");
            await _underTest.AddEmployeeAsync(destEmp);


            var destEmail = new EmailAddress { Value = "destEmail@another.com" };

            var appInvite = new ApplicationInvitation
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Application = MiseAppTypes.UnitTests,
                DestinationEmail = destEmail,
                DestinationEmployeeID = null,
                InvitingEmployeeID = invitingEmp.ID,
                InvitingEmployeeName = invitingEmp.Name,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                RestaurantName = rest.Name,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100 },
                Status = InvitationStatus.Created
            };

            await _underTest.AddApplicationInvitiation(appInvite);

            var noChange = new ApplicationInvitation
            {
                ID = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Application = MiseAppTypes.UnitTests,
                DestinationEmail = destEmail,
                DestinationEmployeeID = null,
                InvitingEmployeeID = invitingEmp.ID,
                InvitingEmployeeName = invitingEmp.Name,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                RestaurantName = rest.Name,
                Revision = new EventID {AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100},
                Status = InvitationStatus.Created
            };
            await _underTest.AddApplicationInvitiation(noChange);

            //UPDATE
            var updatedInvite = new ApplicationInvitation
            {
                ID = appInvite.ID,
                CreatedDate = DateTime.UtcNow,
                Application = MiseAppTypes.UnitTests,
                DestinationEmail = new EmailAddress { Value = "updated@mise.com" },
                DestinationEmployeeID = destEmp.ID,
                InvitingEmployeeID = invitingEmp.ID,
                InvitingEmployeeName = invitingEmp.Name,
                LastUpdatedDate = DateTime.UtcNow,
                RestaurantID = rest.ID,
                RestaurantName = rest.Name,
                Revision = new EventID { AppInstanceCode = MiseAppTypes.UnitTests, OrderingID = 100 },
                Status = InvitationStatus.Rejected
            };

            await _underTest.UpdateApplicationInvitation(updatedInvite);

            //GET BY EMAIL
            var getForRest = (await _underTest.GetApplicationInvitations()).ToList();
            var item = getForRest.First(ai => ai.ID == updatedInvite.ID);
            Assert.AreEqual(updatedInvite.ID, item.ID, "ID");
            Assert.AreEqual(updatedInvite.Status, item.Status, "Status updated");
            Assert.NotNull(updatedInvite.DestinationEmail);
            Assert.AreEqual("updated@mise.com", updatedInvite.DestinationEmail.Value, "Dest email updated");
            Assert.AreEqual(updatedInvite.RestaurantID, item.RestaurantID, "Restauarnt ID");
            Assert.AreEqual(updatedInvite.RestaurantName, item.RestaurantName, "Restaurant Name");

            Assert.AreEqual(updatedInvite.DestinationEmployeeID, item.DestinationEmployeeID, "dest ID");
            Assert.AreEqual(updatedInvite.InvitingEmployeeID, item.InvitingEmployeeID);
            Assert.AreEqual(updatedInvite.InvitingEmployeeName, item.InvitingEmployeeName);

            var notChanged = getForRest.First(ai => ai.ID == noChange.ID);
            Assert.AreEqual(noChange.ID, notChanged.ID, "ID");
            Assert.AreEqual(noChange.Status, notChanged.Status, "Status not updated");
            Assert.NotNull(noChange.DestinationEmail);
            Assert.AreEqual(noChange.DestinationEmail,notChanged.DestinationEmail, "Dest email not updated");
            Assert.AreEqual(noChange.RestaurantID, notChanged.RestaurantID, "Restauarnt ID");
            Assert.AreEqual(noChange.RestaurantName, notChanged.RestaurantName, "Restaurant Name");

            Assert.False(notChanged.DestinationEmployeeID.HasValue, "dest ID");
            Assert.AreEqual(noChange.InvitingEmployeeID, notChanged.InvitingEmployeeID);
            Assert.AreEqual(noChange.InvitingEmployeeName, notChanged.InvitingEmployeeName);
        }
    }
}
