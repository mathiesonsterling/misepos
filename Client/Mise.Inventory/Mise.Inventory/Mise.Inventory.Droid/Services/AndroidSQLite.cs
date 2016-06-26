using System.Threading.Tasks;
using Mise.Core.Client.Services;

namespace Mise.Inventory.Droid.Services
{
	public class AndroidSQLite : ISQLite
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
			const string SQLITE_FILENAME = "StockboyLocalStore.db3";
			var documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			var path = System.IO.Path.Combine (documentsPath, SQLITE_FILENAME);

			return path;
		}
		#endregion
	}
}

