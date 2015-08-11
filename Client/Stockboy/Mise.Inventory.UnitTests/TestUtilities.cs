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
using SQLite;


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
		    private static readonly object Locker = new object();

		    #region ISQLite implementation
			public SQLiteConnection GetDatabase ()
			{
			    lock (Locker)
			    {
			        string FILENAME = GetLocalFilename ();
			        if (File.Exists(FILENAME))
			        {
			            File.Delete(FILENAME);
			        }

			        var db = new SQLiteConnection(FILENAME);
                    return db;
			    }

			}

			public string GetLocalFilename ()
			{
				return "TestDB.db";
			}

			public Task DeleteDatabaseFile ()
			{
				throw new NotImplementedException ();
			}
			#endregion
		}
    }
}
