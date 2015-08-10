using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Common.Services.Implementation.DAL;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Entities.People.Events;
using NUnit.Framework;
using Mise.Core.Common.UnitTests.Tools;
using Moq;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.People;
using Mise.Core.Client.Repositories;
using Mise.Core.ValueItems;
using Mise.Core.Common.Events;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities;


namespace Mise.Core.Client.UnitTests.Repositories
{
	[TestFixture]
	public class ClientEmployeeRepositoryTests
	{
		/// <summary>
		/// Since bad login might not have a selected event, we need to make sure the repository works with it
		/// </summary>
		[Test]
		public void EmployeeRepositoryHandlesBadLoginEvent(){
			var blEvent = new BadLoginAttemptEvent {
				EventOrderingID = new EventID (),
				EmployeeID = new Guid (),
				PasscodeGiven = "2293"
			};

			var dal = new Mock<IClientDAL> ();
			var service = MockingTools.GetTerminalService ();
			var logger = new Mock<ILogger> ();
            var repository = new ClientEmployeeRepository(service.Object, dal.Object, logger.Object, MockingTools.GetResendEventsService().Object);

			//ACT
			var res = repository.ApplyEvent (blEvent);

			//ASSERT
			Assert.Null (res, "Employee is null");
		}
	}
}

