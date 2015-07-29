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

using Mise.Inventory.Services;
using System.IO;
using Mise.Core.Services;


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

		public static IClientDAL GetTestSQLDB(){
			var connService = new TestingSQLConnection ();

			var logger = new Mock<ILogger> ();
			var serializer = new Mise.Core.Common.Services.Implementation.Serialization.JsonNetSerializer ();
			return new Mise.Inventory.Services.Implementation.SQLiteClietDAL (logger.Object, serializer, connService);
		}

		private class TestingSQLConnection : ISQLite
		{
			#region ISQLite implementation
			public SQLite.SQLiteConnection GetDatabase ()
			{
				var file = "TestDB.db";
				if(File.Exists (file)){
					File.Delete (file);
				}

				var db = new SQLite.SQLiteConnection (file);

				return db;
			}
			#endregion
			
		}
    }
}
