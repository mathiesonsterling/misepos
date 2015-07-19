using System;
using System.Collections.Generic;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Menu
{
	public class MenuItemModifierGroup : RestaurantEntityBase
	{
		public MenuItemModifierGroup ()
		{
			Modifiers = new List<MenuItemModifier> ();
		}

		public string DisplayName{get;set;}
		/// <summary>
		/// If true, you can only have one option from this group.
		/// </summary>
		public bool Exclusive { get; set;}

		/// <summary>
		/// If true, server MUST pick at least one value if Default is not set
		/// </summary>
		/// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
		public bool Required{ get; set;}

		/// <summary>
		/// If not null or empty, we should use this item until told differently
		/// </summary>
		/// <value>The default item's ID (is in the modifiers collection).</value>
		public Guid? DefaultItemID{ get; set;}

		public List<MenuItemModifier> Modifiers{ get; set;}

	}
}

