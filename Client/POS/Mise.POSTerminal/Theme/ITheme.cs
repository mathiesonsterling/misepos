using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Core.Entities.Check;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;

namespace Mise.POSTerminal.Theme
{
	public interface ITheme
	{

		/// <summary>
		/// Collection of all colors
		/// </summary>
		/// <value>The colors.</value>
		IEnumerable<Color> Colors{ get; }

		Color GetColorForEmployee(IEmployee emp);

		Color GetColorForEmployee(Guid empID, string preferredColorName = "");

		/// <summary>
		/// Lets us load the color that an employee will use.  Can be used to transfer between Xamarin and Android colors
		/// </summary>
		/// <param name="id">Emmployee id to set for.</param>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// 
		void LoadColorForEmployee(Guid id, int r, int g, int b);

		Color GetColorForCheck(ICheck check);

		Color GetColorForPaymentStatus(CheckPaymentStatus status);

		Color TextColor{ get; }

		Color SelectedTextColor{ get; }

		Color SelectedBackgroundColor{ get; }

		Color CategoryColor{ get; }

		//int ButtonFontSize{get;}
		Font ButtonFont{ get; }

		int BorderRadius{ get; }

		int EmployeeButtonHeight{ get; }

		int CheckButtonHeight{ get; }

		int NumChecksPerRow{ get; }

		int CategoryButtonHeight{ get; }

		int OrderItemFontSize{ get; }

		Color OrderItemFontColor{ get; }

		int ModifierFontSize{ get; }

		Color ModifierFontColor{ get; }

		Color OrderDividerLineColor{ get; }

		int MenuItemButtonPadding{ get; }

		int MenuItemButtonWidth{ get; }

		int MenuItemButtonFontSize{ get; }

		/// <summary>
		/// Color the HotItem buttons should be
		/// </summary>
		/// <value>The color of the hot item.</value>
		Color HotItemColor{ get; }

		/// <summary>
		/// Gets the color of the mise item.
		/// </summary>
		/// <value>The color of the mise item.</value>
		Color MiseItemColor{ get; }

		/// <summary>
		/// Color for items that are in a subcategory of our currently selected category
		/// </summary>
		/// <value>The color of the sub cat item.</value>
		Color SubCatItemColor{ get; }

		Color MenuItemColor{ get; }

	}
}

