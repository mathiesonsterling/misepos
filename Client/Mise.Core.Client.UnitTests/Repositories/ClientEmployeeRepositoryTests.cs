using System;
using Mise.Core.Common.Events.Employee;
using NUnit.Framework;
using Mise.Core.Common.UnitTests.Tools;
using Moq;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.People;
using Mise.Core.Client.Repositories;
using Mise.Core.ValueItems;


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
				EventOrder = new EventID (),
				EmployeeID = new Guid (),
				PasscodeGiven = "2293"
			};
					
			var service = MockingTools.GetTerminalService ();
			var logger = new Mock<ILogger> ();
            var repository = new ClientEmployeeRepository(service.Object, logger.Object);

			//ACT
			var res = repository.ApplyEvent (blEvent);

			//ASSERT
			Assert.Null (res, "Employee is null");
		}
	}
}

