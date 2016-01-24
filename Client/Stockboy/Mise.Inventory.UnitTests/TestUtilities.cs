using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Services.UtilityServices;
using Moq;
using NUnit.Framework;

using Mise.Inventory.Services;
using System.IO;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Services;


namespace Mise.Inventory.UnitTests
{
    public static class TestUtilities
    {
        public static Mock<IClientDAL> GetDALWithoutResends()
        {
            var moq = new Mock<IClientDAL>();
            moq.Setup(d => d.GetUnsentEvents())
                .Returns(Task.FromResult(new List<EventDataTransportObject>().AsEnumerable()));
            return moq;
        }
    }   
}
