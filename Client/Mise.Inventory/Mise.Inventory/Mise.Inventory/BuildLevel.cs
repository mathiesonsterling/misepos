using System;

namespace Mise.Inventory
{
	/// <summary>
	/// Are we a QA, Dev, production, or demo build?
	/// </summary>
	public enum BuildLevel
	{
		Demo,
		Debugging,
		Development,
		QA,
		Production
	}
}

