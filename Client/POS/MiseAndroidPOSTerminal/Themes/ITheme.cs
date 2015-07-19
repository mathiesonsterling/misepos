using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Menu;
namespace MiseAndroidPOSTerminal.Themes
{
	/// <summary>
	/// Determines all spacing, colors, and other variable UI elements to allow us to do theming
	/// </summary>
	public interface IMiseAndroidTheme
	{
		/// <summary>
		/// Lets our theme know about our width and height.  Used since activities know this but apps dont
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		void SetScreenDimensions (int width, int height);

		/// <summary>
		/// Centralize button creation, so we can add styles and the like
		/// </summary>
		/// <returns>The button.</returns>
		/// <param name="context">Context.</param>
		Button CreateButton (Context context, string text, Color color);

		/// <summary>
		/// Create a button, with a set width and height rather than minimum
		/// </summary>
		/// <returns>The button.</returns>
		/// <param name="context">Context.</param>
		/// <param name="color"></param> 
		/// <param name="test">Test.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		Button CreateButton (Context context, string test, Color color, int width, int height);
		Button CreateButton (Context context, string text, Color color, int width, int height,
		                    int horizontalPadding, int verticalPadding);

		Button CreateMenuItemButton (Context context, MenuItem item, bool inSubcat);

		Button CreateHotItemButton (Context context, MenuItem item);

		Button CreateMiseItemButton (Context context, MenuItem item);

		ImageButton CreateImageButton (Context context, string resourceName, Color bgColor, int width, int height);

		/// <summary>
		/// Lets the theme adjust an already created button
		/// </summary>
		/// <param name="button">Button.</param>
		Button ThemeButton (Button button);
		Button ThemeButton (Button button, int buttonPadding);

		/// <summary>
		/// Puts a button or other text view into the visual state of being selected
		/// </summary>
		/// <returns>The button as selected.</returns>
		TextView MarkAsSelected (TextView view);

		TextView MarkOrderItemAsSelected (TextView oiTextView);

		void SetButtonEnabled (Button button, bool enabled);
		void MarkButtonAsEnabled (Button button);
		void MarkButtonAsDisabled (Button button);

		/// <summary>
		/// Get the color the check should be
		/// </summary>
		/// <returns>The tab button color.</returns>
		/// <param name="check">Check.</param>
		Color GetCheckButtonColor (ICheck check);

		/// <summary>
		/// Create a horizontal row
		/// </summary>
		/// <returns>The row for grid.</returns>
		/// <param name="context">Context.</param>
		LinearLayout CreateRowForGrid (Context context);
		LinearLayout CreateRowForGrid(Context context, int buttonPaddingInPxHor);

		/// <summary>
		/// Create a vertical row
		/// </summary>
		/// <returns>The column.</returns>
		/// <param name="context">Context.</param>
		LinearLayout CreateColumn (Context context);

		/// <summary>
		/// Create an edit text
		/// </summary>
		/// <returns>The edit text.</returns>
		/// <param name="context">Context.</param>
		EditText CreateEditText (Context context);
		/// <summary>
		/// Given a column position in the grid, give the correct background for it
		/// </summary>
		/// <returns>The color for column position.</returns>
		/// <param name="columnIndex">Column index.</param>
		Color GetColorForColumnPosition (int columnIndex);

		/// <summary>
		/// Each employee gets a color, and color codes their items that way
		/// </summary>
		/// <returns>The color for employee.</returns>
		/// <param name="employeeID">Employee I.</param>
		Color GetColorForEmployee (Guid employeeID);


		IEnumerable<Tuple<Guid, int, int, int>> ExportEmployeeColors();


		/// <summary>
		/// Color of any items in the main category
		/// </summary>
		/// <value>The color of the main category.</value>
		Color MainCategoryColor{ get; }

		/// <summary>
		/// Color of any items in the sub category
		/// </summary>
		/// <value>The color of the sub category.</value>
		Color SubCategoryColor{ get; }

		Color CategoryRowTextColor{ get; }
		Color UnknownCategoryColor{ get; }
		Color WindowBackground{ get; }

		int ButtonPadding{ get; }

		Typeface Typeface{ get;}
		TypefaceStyle TypefaceStyle{ get; }
		Color DefaultTextColor{get;}
		Color DisabledTextColor{get;}

		int LoginButtonWidth{ get; }
		int LoginButtonHeight{ get;  }
		Color LoginButtonBackground{get;}

		int TopRowOnOrderHeight{get;}

		int ClockOutButtonWidth{get;}
		int ClockOutButtonHeight{ get; }
		Color ClockOutButtonBackground{get;}

		int EmployeeButtonWidth{ get; }
		int EmployeeButtonHeight{ get; }
		Color EmployeeButtonBackground{ get;}
		Color EmployeeButtonTextColor{ get; }
		Color LastSelectedEmployeeButtonColor{ get;}
		Color LastSelectedEmployeeButtonTextColor{ get;}

		int EmployeeOptionsColumnWidth{get;}

		int AddTabConfirmButtonWidth{get;}
		int AddTabButtonWidth{get;}
		int AddTabButtonHeight{get;}
		Color AddTabButtonColor{get;}

		int EmpOptionsButtonWidth{get;}
		int EmpOptionsButtonHeight{get;}
		Color EmpOptionsButtonColor{get;}

		int NumTabColumns{ get; }
		int TabButtonWidth{ get;}
		int TabButtonHeight{ get;}

		int NumMenuItemColumns{ get;}
		int NumberOfHotItems{get;}
		int MaxNumMiseItems{get;}

		int MenuItemButtonWidth{ get;}
		int MenuItemButtonHeight{ get;}
		int MenuItemButtonPadding{ get; }

		int OrdersScreenCenterColumnWidth{get;}
		int OrdersScreenModifierTitleWidth{get;}

		int ModifierItemWidth{get;}
		int ModifierMemoWidth{ get; }

		int ModifiersTopTextSize{get;}

		int MaxTabNameLength {get;}

		Color ModifierTextColor{ get; }

		Color HotItemColor{ get;}

		Color MiseItemColor{ get; }

		/// <summary>
		/// Color for modifiers that need to be hit, but don't have a default
		/// </summary>
		/// <value>The color of the required modifier button.</value>
		Color RequiredModifierButtonColor{ get;}
		Color PossibleModifierButtonColor{ get;}
		Color CommitModifiersButtonColor{ get;}

		/// <summary>
		/// Button used for all 'cancel' functions
		/// </summary>
		/// <value><c>true</c> if this instance cancel button color; otherwise, <c>false</c>.</value>
		Color CancelButtonColor{ get;}

		/// <summary>
		/// Generic confirm buttons colored this
		/// </summary>
		/// <value>The color of the OK button.</value>
		Color OKButtonColor{get;}

		int CategoryMenuWidth{get;}
		int CategoryMenuHeight{get;}
		int CategoryButtonWidth{ get;}
		int CategoryNavButtonWidth{get;}
		int CategoryButtonHeight{get;}
		Color CategoryButtonColor{ get;}
		Color CategoryTextColor{ get;}
		int CategoryTextSize{get;}
		int CategoryTopTextSize{get;}
		int CategoryColumnHeight{ get;}
	
		int OrderScreenCenterScrollerHeight{get;}
		int PaymentsPaymentSpecificAreaScrollerHeight{ get; }

		int PaymentsPaymentListScrollerHeight{ get; }

		Color CloseOrderButtonColor{ get;}

		int OrdersScreenCommandBarHeight{get;}
		int OrderCommandDividerHeight{get;}
		int CommandButtonWidth{get;}

		Color SendCheckButtonColor{ get;}
		Color ExtrasColor{get;}

		int OrderMiddleRowHeight{get;}
		int OrderItemPanelWidth{ get; }

		int CheckNameTextSize{ get; }

		int OrderItemTextSize{ get;}
		int OrderItemModifierTextSize{ get;}
		Color OrderItemTextColor{ get;}
		Color OrderItemTextColorSent{get;}
		Color OrderItemTextColorUnsent{ get; }
		Color OrderItemListBackgroundColor{get;}

		Color VoidItemBackgroundColor{get;}

		int CashCloseTotalTextSize{get;}
		int NumberOfCashButtons{get;}

		Color NoSaleButtonColor{get;}

		Color CategoryHomeColor{ get; }

		Color CategoryUpColor{ get; }

		#region Payments
		int PaymentsScreenMiddleColumnWidth{get;}
		int PaymentsScreenUpperMiddleHeight{ get; }

		int PaymentsScreenRightColumnWidth{get;}

		int ReopenButtonHeight{get;}
		Color ReopenButtonColor{get;}

		int CashButtonHeight{ get;}
		Color CashButtonColor{ get; }

		int CreditButtonHeight{get;}
		Color CreditButtonColor{get;}

		int AddGratuityHeight{get;}
		Color AddGratuityColor{get;}

		int PaymentsAmountTextSize{get;}
		int PaymentsCancelButtonHeight{get;}
		int PaymentsSendButtonHeight{get;}
		Color PaymentsSentButtonColor{get;}

		int PayCheckHeight{get;}
		Color PayCheckColor{get;}

		Color PayPercentageColor{get;}
		int NumberCashPaymentChoices{get;}

		Color CreditCardTextColor{get;}
		int CreditCardTextSize{get;}

		int PaymentSpaceHeight{get;}
		int PaymentTextSize{get;}
		int PaymentTypeWidth{get;}
		int PaymentAmountWidth{get;}
		/// <summary>
		/// What color the text showing the payment type is
		/// </summary>
		/// <value>The color of the payment type.</value>
		Color GetPaymentTypeColor (PaymentType type);

		int PaymentItemButtonWidth{get;}

		int PaymentAddCreditCardIndent{get;}

		Color AddTipColor{ get;}
		#endregion

		#region Comps
		/// <summary>
		/// Color used by all comp function buttons
		/// </summary>
		/// <value>The color of the comp.</value>
		Color CompColor{get;}

		int CompItemButtonWidth{get;}
		int CompItemButtonHeight{get;}
		#endregion

		#region Discounts
		/// <summary>
		/// Both text on OI List and buttons will match here
		/// </summary>
		/// <value>The color of the discount.</value>
		Color DiscountColor{get;}

		Color GratuityColor{get;}
		#endregion
	}
}

