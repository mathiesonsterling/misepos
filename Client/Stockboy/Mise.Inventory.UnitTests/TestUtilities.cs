using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Services.WebServices;
using Moq;
using NUnit.Framework;

namespace Mise.Inventory.UnitTests
{
    public static class TestUtilities
    {
        public static Mock<IResendEventsWebService> GetResendService()
        {
            var moq = new Mock<IResendEventsWebService>();
            moq.Setup(s => s.ResendEvents(It.IsAny<ICollection<IEntityEventBase>>()))
                .Returns(Task.FromResult(true));

            return moq;
        }

        public static Mock<IClientDAL> GetDALWithoutResends()
        {
            var moq = new Mock<IClientDAL>();
            moq.Setup(d => d.GetUnsentEvents())
                .Returns(Task.FromResult(new List<EventDataTransportObject>().AsEnumerable()));
            return moq;
        }
    }
}
