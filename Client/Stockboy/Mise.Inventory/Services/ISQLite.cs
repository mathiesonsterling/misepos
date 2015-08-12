using System;

using SQLite;
using System.Threading.Tasks;


namespace Mise.Inventory.Services
{
	/// <summary>
	/// Provides platform specific information on constructing the SQLite DB
	/// </summary>
	public interface ISQLite
	{
		string GetLocalFilename();

		Task DeleteDatabaseFile();
	}
}

