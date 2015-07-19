using System;
using System.Collections.Generic;
using System.Linq;

using Android.Database.Sqlite;
using Android.Content;

using Mise.Core.Services;
using Mise.Core.Common.Services;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.Entities;
using Android.Database;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Entities.MenuItems;
using  Mise.Core.Entities.Menu;
using System.Threading.Tasks;
using Android.Nfc;
using Android.Widget;


namespace Mise.AndroidCommon.Services
{

	public class AndroidSQLLiteDAL : IClientDAL
	{
		class MiseDatabaseHelper : SQLiteOpenHelper{
			const string DATABASE_NAME = "miseTerminal";
			const int DATABASE_VERSION = 1;

			public const string ENTITIESTABLENAME = "Entities";
			public const string EVENTSTABLENAME = "Events";
			public MiseDatabaseHelper(Context context) : base(context, DATABASE_NAME, null, DATABASE_VERSION)
			{
			}
				
			public override void OnCreate(SQLiteDatabase db)
			{
				db.ExecSQL("CREATE TABLE "+ENTITIESTABLENAME+@" (
		          Id NVARCHAR(16) PRIMARY KEY,
					Status int NOT NULL,
		          Revision BIGINT NOT NULL,
		          Type TEXT NOT NULL,
				  JSON TEXT NOT NULL
		        )");

				db.ExecSQL(@"CREATE TABLE "+EVENTSTABLENAME+@" (
		          Id NVARCHAR(16) PRIMARY KEY,
				  Status int NOT NULL,
		          Revision BIGINT NOT NULL,
		          Type TEXT NOT NULL,
				  JSON TEXT NOT NULL
		        )");
			}
				
			public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
			{
				db.ExecSQL("DROP TABLE IF EXISTS " + ENTITIESTABLENAME);
				db.ExecSQL("DROP TABLE IF EXISTS " + EVENTSTABLENAME);

				OnCreate(db);
			}

		}

		readonly MiseDatabaseHelper _helper;
		readonly object _writeLocker;
		public AndroidSQLLiteDAL (Context context, ILogger logger, IJSONSerializer serializer)
		{
			_helper = new MiseDatabaseHelper (context);
			Logger = logger;
			Serializer = serializer;
			_writeLocker = new object ();
		}
			
		public IJSONSerializer Serializer {
			get;
			set;
		}

		public ILogger Logger {
			get;
			set;
		}

		#region IClientDAL implementation

		public Task CleanItemsBefore (DateTimeOffset minDate, int maxNumberEntites = 2147483647, int maxNumberEvents = 2147483647)
		{
			throw new NotImplementedException ();
		}

		public bool UpdateEventStatuses (IEnumerable<IEntityEventBase> events, ItemCacheStatus status)
		{
			try{
				lock(_writeLocker){
					_helper.WritableDatabase.BeginTransactionNonExclusive ();

					foreach (var item in events) {
						var vals = new ContentValues();
						vals.Put ("Status", (int)status);
						_helper.WritableDatabase.Update(MiseDatabaseHelper.ENTITIESTABLENAME, vals, "ID = '" + item.ID+"'", null);
					}

					_helper.WritableDatabase.EndTransaction ();

					return true;
				}
			} catch(Exception e){
				Logger.HandleException (e);
				return false;
			}
		}

		public Task<bool> UpdateEventStatusesAsync (IEnumerable<IEntityEventBase> events, ItemCacheStatus status)
		{
			return Task.Factory.StartNew (() => UpdateEventStatuses (events, status));
		}

		public Task<bool> StoreEventsAsync (IEnumerable<IEntityEventBase> events)
		{
			return Task.Factory.StartNew (() => StoreEvents (events));
		}


		public bool StoreEvents (IEnumerable<IEntityEventBase> events)
		{
			try{
				lock(_writeLocker){
					_helper.WritableDatabase.BeginTransactionNonExclusive ();
					foreach(var ev in events){
						var vals = new ContentValues ();
						vals.Put("ID", ev.ID.ToString ());
						vals.Put("Status", (int)ItemCacheStatus.TerminalDB);
						vals.Put("Revision", ev.EventOrderingID.OrderingID);
						vals.Put("Type", ev.GetType ().ToString ());
						vals.Put("JSON", Serializer.Serialize (ev));

						 _helper.WritableDatabase.Insert (MiseDatabaseHelper.EVENTSTABLENAME, null, vals);
						//TODO check on our res
					}
					_helper.WritableDatabase.EndTransaction ();
					return true;
				}
			} catch(Exception e){
				Logger.HandleException (e);
				return false;
			}
		}

		public Task<bool> UpsertEntitiesAsync (IEnumerable<IEntityBase> entities)
		{
			return Task.Factory.StartNew (() => UpsertEntities (entities));
		}

		public bool UpsertEntities (IEnumerable<IEntityBase> entities)
		{
			var res = new List<IEntityBase> ();
			lock (_writeLocker) {
				foreach (var ent in entities) {
					try{
						//see if it exists
						Logger.Log ("Upserting entity of type " + ent.GetType (), LogLevel.Debug);

						var findCursor = _helper.ReadableDatabase.Query (MiseDatabaseHelper.ENTITIESTABLENAME,
							                new []{ "ID" }, "ID = '" + ent.ID + "'", null, null, null, null);

						bool found = false;
						while (findCursor.MoveToNext ()) {
							found = true;
							break;
						}

						var vals = new ContentValues ();
						vals.Put ("ID", ent.ID.ToString ());
						vals.Put ("Status", (int)ItemCacheStatus.TerminalDB);
						vals.Put ("Revision", ent.Revision.OrderingID);
						vals.Put ("Type", ent.GetType ().ToString ());

						string json;
						json = Serializer.Serialize (ent);
						vals.Put ("JSON", json);

						if (found) {
							//update
							Logger.Log ("Found entity for ID " + ent.ID + ", updating", LogLevel.Debug);
							_helper.WritableDatabase.Update (MiseDatabaseHelper.ENTITIESTABLENAME, vals, "ID = '" + ent.ID + "'", null);
						} else {
							//insert
							Logger.Log ("Inserting new entity for " + ent.ID, LogLevel.Debug);
							_helper.WritableDatabase.Insert (MiseDatabaseHelper.ENTITIESTABLENAME, null, vals);
						}

						res.Add(ent);
					} catch(Exception ex){
						Logger.HandleException(ex);
					}
				}
			}
			Logger.Log ("All items upserted", LogLevel.Debug);
			return res.Any();
		}

		public IEnumerable<T> GetEntities<T>() where T: class, IEntityBase
		{
			switch(typeof(T).Name){
			case "ICheck":
				return (IEnumerable<T>)GetChecks ();
			case "IEmployee":
				return (IEnumerable<T>)GetEmployees ();
			case "IMiseTerminalDevice":
				return (IEnumerable<T>)(new List<IMiseTerminalDevice>{GetDeviceSettings ()});
			case "Menu":
				return (IEnumerable<T>)(new List<Menu>{ GetMenu () });
			default:
				throw new NotImplementedException ("Don't know how to store " + typeof(T).Name);
			}
				
		}

		public Task<IEnumerable<T>> GetEntitiesAsync<T> () where T : class, IEntityBase
		{
			return Task.Factory.StartNew (() => GetEntities<T> ());
		}
			

		IEnumerable<ICheck> GetChecks ()
		{
			Logger.Log ("Getting checks from DB", LogLevel.Debug);
			var cursor = _helper.ReadableDatabase.Query (MiseDatabaseHelper.ENTITIESTABLENAME,
				null, "Type LIKE '%ICheck%'" , null, null, null, null);

			while(cursor.MoveToNext ()){
				yield return Map<RestaurantCheck> (cursor);
			}
		}
			
		IEnumerable<IEmployee> GetEmployees ()
		{
			var cursor = _helper.ReadableDatabase.Query (MiseDatabaseHelper.ENTITIESTABLENAME,
				null, "Type LIKE '%Employee%'" , null, null, null, null);

			while(cursor.MoveToNext()){
				yield return Map<Employee> (cursor);
			}
		}
			

		IMiseTerminalDevice GetDeviceSettings ()
		{
			try{
				var cursor = _helper.ReadableDatabase.Query (MiseDatabaseHelper.ENTITIESTABLENAME, null,
					             "Type LIKE %MiseTerminalDevice%", null, null, null, null);
				while(cursor.MoveToNext ()){
					return Map<MiseTerminalDevice> (cursor);
				}
			} catch(Exception e){
				Logger.HandleException (e);
			}
			return null;
		}
			
		Menu GetMenu ()
		{
			var cursor = _helper.ReadableDatabase.Query (MiseDatabaseHelper.ENTITIESTABLENAME, null,
				             "Type LIKE %Menu%", null, null, null, null);
			while(cursor.MoveToNext ()){
				return Map<Menu> (cursor);
			}

			return null;
		}
			
		#endregion

		T Map<T>(ICursor cursor) where T:class{
			var json = cursor.GetString (4);
			Logger.Log ("Got entity json - " + json, LogLevel.Debug);
			return Serializer.Deserialize<T> (json);
		}

	}
}

