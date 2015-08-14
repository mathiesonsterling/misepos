using System;
using System.Threading.Tasks;
using Mise.Inventory.Services;

namespace Mise.Inventory.iOS.Services
{
	public class iOSSQLite : ISQLite
	{
		#region ISQLite implementation

		public Task DeleteDatabaseFile ()
		{
			var filename = GetLocalFilename ();

			if (System.IO.File.Exists (filename)) {
				System.IO.File.Delete (filename);
			}

			return Task.FromResult (true);
		}

		public string GetLocalFilename(){
			const string sqliteFilename = "MiseStockboy.db3";
			//TODO shot in the dark for now
			return sqliteFilename;
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			var path = System.IO.Path.Combine (documentsPath, sqliteFilename);

			return path;
		}
		#endregion
	}
}

