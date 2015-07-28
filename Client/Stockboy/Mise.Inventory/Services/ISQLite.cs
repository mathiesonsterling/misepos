using System;

using SQLite;
namespace Mise.Inventory.Services
{
	/// <summary>
	/// Provides platform specific information on constructing the SQLite DB
	/// </summary>
	public interface ISQLite
	{
		SQLiteConnection GetDatabase();
	}
}

