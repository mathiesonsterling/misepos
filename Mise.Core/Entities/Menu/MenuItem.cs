using System.Collections.Generic;
using System.Linq;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Menu
{
	public class MenuItem : RestaurantEntityBase
	{
		public MenuItem(){
			PossibleModifiers = new List<MenuItemModifierGroup> ();
			Destinations = new List<OrderDestination> ();
		}
			


		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// List of how popular this item is. Higher is more
		/// </summary>
		/// <value>The display weight.</value>
		public int DisplayWeight {
			get;
			set;
		}


		private string _oiListName = string.Empty;
		public string OIListName{ get{ return _oiListName != string.Empty ? _oiListName : Name; } set{_oiListName = value;}}

		private string _buttonName = string.Empty;
		public string ButtonName{ get{ return _buttonName != string.Empty ? _buttonName : Name;} set{_buttonName = value;}}

		private string _printerName = string.Empty;
		public string PrinterName{ get{ return _printerName != string.Empty ? _printerName : Name;}set{_printerName = value;}}

		public Money Price
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}
			

		public IEnumerable<MenuItemModifier> GetDefaultModifiers() {
				var ids = PossibleModifiers.Where (pm => pm.DefaultItemID.HasValue).Select(pm => pm.DefaultItemID.Value).ToList();
				var mods = PossibleModifiers.SelectMany(pm => pm.Modifiers).Where(m => ids.Contains(m.ID));
				return mods;
		}
			
		public List<MenuItemModifierGroup> PossibleModifiers { get; set; }

		public List<OrderDestination> Destinations {
			get; set;
		}

	}
}


