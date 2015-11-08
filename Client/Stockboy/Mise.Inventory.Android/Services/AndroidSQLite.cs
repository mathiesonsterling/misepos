using System;

using Mise.Inventory.Services;
using Java.IO;
using System.Threading.Tasks;


namespace Mise.Inventory.Android.Services
{
	public class AndroidSQLite : ISQLite
	{
		#region ISQLite implementation
		public SQLite.SQLiteConnection GetDatabase ()
		{
			var path = GetLocalFilename ();

			var conn = new SQLite.SQLiteConnection (path);

			return conn;
		}

		public System.Threading.Tasks.Task DeleteDatabaseFile ()
		{
			var filename = GetLocalFilename ();

			if (System.IO.File.Exists (filename)) {
				System.IO.File.Delete (filename);
			}

			return Task.FromResult (true);
		}

		public string GetLocalFilename(){
			const string sqliteFilename = "StockboyLocalStore.db3";
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			var path = System.IO.Path.Combine (documentsPath, sqliteFilename);

			return path;
		}
		#endregion
	}
}

