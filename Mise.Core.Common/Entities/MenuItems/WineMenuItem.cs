using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.MenuItems
{
	public class WineMenuItem : MenuItem
	{
		/// <summary>
		/// What year the wine was, if set
		/// </summary>
		/// <value>The vintage year.</value>
		public int? VintageYear{ get; set; }

		public string Location{get;set;}
		/// <summary>
		/// Country the wine comes from
		/// </summary>
		/// <value>The country.</value>
		public Country Country{get;set;}
	}
}

