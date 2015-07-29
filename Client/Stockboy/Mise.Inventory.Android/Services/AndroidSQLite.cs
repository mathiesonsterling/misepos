using System;

using Mise.Inventory.Services;
namespace Mise.Inventory.Android.Services
{
	public class AndroidSQLite : ISQLite
	{
		#region ISQLite implementation
		public SQLite.SQLiteConnection GetDatabase ()
		{
			var sqliteFilename = "MiseStockboy.db3";
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			var path = System.IO.Path.Combine (documentsPath, sqliteFilename);

			var conn = new SQLite.SQLiteConnection (path);

			return conn;
		}
		#endregion
	}
}

