using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;

using Android.Util;
using Android.Views.InputMethods;

namespace MiseAndroidPOSTerminal.Themes
{
	public class DefaultTheme : IMiseAndroidTheme
	{

		#region Colors
		static Color Green = new Color(63,218,107);
		static Color Orange = new Color (225, 100, 58);
		static Color Red = new Color (212, 50, 81);
		static Color Blue = new Color (65, 169, 242);
		static Color Yellow = new Color (226, 208, 56);
		static Color Purple = new Color (157, 70, 207);

		static Color Grey = new Color (70, 70, 70);
		static Color LightGray = new Color (235, 235, 235);

		static Color Pink = new Color(255, 123, 206);
		static Color Black = new Color(35,35,35);
		static Color CategoryBlack = new Color (51, 51, 51);

		static Color ModifierGrey = new Color (99, 99, 99);

		static Color White = Color.White;
		static Color BluishWhite = new Color (210, 236, 255);

		IList<Color> _allEmployeeColors = new List<Color>{
			Orange,
			Yellow,
			Purple,
			Green,
		};
		#endregion

		/// <summary>
		/// Given a column position in the grid, give the correct background for it
		/// </summary>
		/// <returns>The color for column position.</returns>
		/// <param name="columnIndex">Column index.</param>
		public Color GetColorForColumnPosition(int columnIndex){
			switch (columnIndex) {
			case 0:
				return Orange;
			case 1: 
				return Pink;
			case 2:
				return Yellow;
			case 3:
				return Purple;
			default:
				return Grey;
			}
		}

		readonly Dictionary<string, Color> _categoryColors = new Dictionary<string, Color> ();

		public Color GetCheckButtonColor(ICheck check){
			Color tabColor = Black;
			//find which color to get
			switch (check.PaymentStatus) {
			case CheckPaymentStatus.Open:
				tabColor = GetColorForEmployee (check.LastTouchedServerID);
				break;
			case CheckPaymentStatus.Closing:
				tabColor = Purple;
				break;
			case CheckPaymentStatus.PaymentRejected:
				tabColor = Red;
				break;
			case CheckPaymentStatus.PaymentApprovedWithoutTip:
				tabColor = AddTipColor;
				break;
			}

			return tabColor;
		}

		public Button CreateButton (Context context, string text, Color color){
			var butt = new Button (context){
				Text = text
			};
			butt.SetBackgroundColor (color);
			return ThemeButton(butt);
		}
			
		public Button ThemeButton (Button butt){
			return ThemeButton (butt, ButtonPadding);
		}

		public Button ThemeButton(Button butt, int buttonPadding){
			var lp = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			lp.SetMargins (buttonPadding, buttonPadding, buttonPadding, buttonPadding);
			butt.LayoutParameters = lp;

			butt.SetTextColor (DefaultTextColor);
			butt.SetTypeface (Typeface, TypefaceStyle);
			butt.SetPadding (buttonPadding, buttonPadding, buttonPadding, buttonPadding);

			butt.SetTextSize (ComplexUnitType.Pt, ButtonFontSize);
			return butt;
		}

		public TextView MarkAsSelected(TextView view){
			view.SetBackgroundColor (SelectedButtonColor);
			view.SetTextColor (SelectedButtonTextColor);

			return view;
		}

		public TextView MarkOrderItemAsSelected(TextView oiTextView){
			var view = MarkAsSelected (oiTextView);
			view.SetTextColor (Green);

			return view;
		}

		public void SetButtonEnabled (Button button, bool enabled){
			if(enabled){
				MarkButtonAsEnabled (button);
			} else {
				MarkButtonAsDisabled (button);
			}
		}

		public void MarkButtonAsEnabled(Button button){
			if (button != null) {
				button.Enabled = true;
				button.SetTextColor (DefaultTextColor); 
			}
		}

		public void MarkButtonAsDisabled(Button button){
			if (button != null) {
				button.Enabled = false;
				button.SetTextColor (DisabledTextColor); 
			}
		}

		public Color LastSelectedEmployeeButtonColor{get{return BluishWhite;}}
		public Color LastSelectedEmployeeButtonTextColor{get{return SelectedButtonTextColor;}}

		public Button CreateButton (Context context, string text, Color color, int width, int height){
			return CreateButton (context, text, color, width, height, ButtonPadding, ButtonPadding);
		}

		public Button CreateButton(Context context, string text, Color color, int width, int height,
			int horizontalMargin, int verticalMargin){
			var butt = new Button (context){
				Text = text
			};

			//TODO change this to use THemeButton instead
			var layoutParams = new LinearLayout.LayoutParams (width, height);
			layoutParams.SetMargins (horizontalMargin, verticalMargin, horizontalMargin, verticalMargin);
			butt.LayoutParameters = layoutParams;

			butt.SetBackgroundColor (color);
			butt.SetTextColor (DefaultTextColor);
			butt.SetTypeface (Typeface, TypefaceStyle);
			butt.SetPadding (ButtonPadding, ButtonPadding, ButtonPadding, ButtonPadding);
			butt.SetTextSize (ComplexUnitType.Pt, ButtonFontSize);
			return butt;
		}

		public ImageButton CreateImageButton (Context context, string resourceName, Color bgColor, int width, int height){
			var butt = new ImageButton (context);

			var layoutParams = new LinearLayout.LayoutParams (width, height);
			layoutParams.SetMargins (ButtonPadding, ButtonPadding, ButtonPadding, ButtonPadding);
			butt.LayoutParameters = layoutParams;

			butt.SetBackgroundColor (bgColor);
			butt.SetPadding (ButtonPadding, ButtonPadding, ButtonPadding, ButtonPadding);

			using (var asset = context.Assets.Open (resourceName)) {
				var bitmap = BitmapFactory.DecodeStream (asset);

				butt.SetImageBitmap (bitmap);
			}
			return butt;
		}

		public Button CreateMenuItemButton (Context context, MenuItem item, bool inSubCat)
		{
			var color = inSubCat ? SubCategoryColor : MainCategoryColor;
			return CreateButton (context, item.ButtonName, color, MenuItemButtonWidth, MenuItemButtonHeight,
				MenuItemButtonPadding, 13);
		}

		public Button CreateHotItemButton (Context context, MenuItem item)
		{
			return CreateButton (context, item.ButtonName, HotItemColor, MenuItemButtonWidth,
				MenuItemButtonHeight, MenuItemButtonPadding, 13);
		}

		public Button CreateMiseItemButton (Context context, MenuItem item){
			return CreateButton (context, item.ButtonName, MiseItemColor, MenuItemButtonWidth, MenuItemButtonHeight,
				MenuItemButtonPadding, 13);
		}
			

		public EditText CreateEditText(Context context){
			var et = new EditText (context);
			et.SetTextSize (ComplexUnitType.Pt, ButtonFontSize);
			et.ImeOptions = ImeAction.Done;

			return et;
		}

		public LinearLayout CreateRowForGrid (Context context){
			return CreateRowForGrid (context, ButtonPadding);
		}

		public LinearLayout CreateRowForGrid(Context context, int buttonPaddingInPx){
			var row = new LinearLayout(context){Orientation = Orientation.Horizontal};
			var lp = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			lp.SetMargins (buttonPaddingInPx, ButtonPadding, buttonPaddingInPx,ButtonPadding);
			row.LayoutParameters = lp;
			return row;
		}

		public LinearLayout CreateColumn (Context context){
			var row = new LinearLayout(context){Orientation = Orientation.Vertical};
			var lp = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			lp.SetMargins (ButtonPadding, ButtonPadding, ButtonPadding, ButtonPadding);
			row.LayoutParameters = lp;
			return row;
		}
			

		#region ITheme implementation
		readonly Dictionary<Guid, Color> _employeeColors = new Dictionary<Guid, Color> ();
		public Color GetColorForEmployee (Guid employeeID)
		{
			try{
				if (_employeeColors.ContainsKey (employeeID)) {
					return _employeeColors [employeeID];
				}

				var currentEmpIndex = _employeeColors.Count % _allEmployeeColors.Count;
				if (currentEmpIndex >= _allEmployeeColors.Count - 1) {
					currentEmpIndex = -1;
				}
				var next = _allEmployeeColors [currentEmpIndex + 1];
				_employeeColors.Add (employeeID, next);

				return next;
			}
			catch(Exception){
				return EmployeeButtonBackground;
			}
		}

		//TODO change to byte all up the stack
		public IEnumerable<Tuple<Guid, int, int, int>> ExportEmployeeColors ()
		{
			foreach (var kv in _employeeColors) {
				yield return new Tuple<Guid, int, int, int> (kv.Key, kv.Value.R, kv.Value.G, kv.Value.B);
			}
		}

		#endregion

		/// <summary>
		/// Lets our theme know about our width and height.  Used since activities know this but apps dont
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public void SetScreenDimensions(int width, int height){
			var _heightMod = (decimal)height / (decimal)750;
			var _widthMod = (decimal)width / (decimal)1280;

			_loginButtonWidth = (int)(170 * _widthMod);
			_loginButtonHeight = (int)(125 * _heightMod);
			_employeeButtonWidth= (int)(250 * _widthMod);
			_employeeButtonHeight = (int)(125 * _heightMod);
			_tabButtonWidth = (int)(150 * _widthMod);
			_tabButtonHeight = (int)(125 * _heightMod);
			_menuItemButtonWidth = (int)(225 * _widthMod);
			_menuItemButtonHeight = (int)(72 * _heightMod);
			_menuItemButtonPadding = (int)(23 * _widthMod);
			_categoryButtonWidth = (int)(270 * _widthMod);
			//_categoryNavButtonWidth = (int)(125 * _widthMod);
			_categoryButtonHeight = (int)(60 * _heightMod);
			//_categoryMenuWidth = (int)(270 * _widthMod);
			_categoryMenuHeight = (int)(60 * _heightMod);
			_categoryColumnHeight = (int)(600 * _heightMod);
			_orderScreenCenterScrollerHeight = (int)(570 * _heightMod);
			_paymentsPaymentSpecificAreaScrollerHeight = (int)(400 * _heightMod);
			_paymentsPaymentListScrollerHeight = (int)(170 * _heightMod);

			_cashButtonHeight = (int)(100 * _heightMod);
			_orderItemPanelWidth = (int)(280 * _widthMod);

			_modItemWidth = (int)(150 * _widthMod);
			_topRowOnOrderHeight = (int)(80 * _heightMod);
			_orderCommandDividerHeight = (int)(60 * _heightMod);

			_middleRowHeight = (int)(670 * _heightMod);
			_addTabConfirmButtonWidth = _loginButtonWidth;

			_paymentScreenUpperMiddleHeight = (int)(250 * _heightMod);
			_paymentScreenMiddleColumnWidth = (int)(790 * _widthMod);
			_paymentSpaceHeight = (int)(50 * _heightMod);

			_paymentAddCreditCardIndent = (int)(50 * _widthMod);

			_ordersScreenCenterColumnWidth = (int)(790 * _widthMod);

			_ordersScreenCommandBarHeight = (int)(75 * _heightMod);
		}

		#region Font sizes
		public int CheckNameTextSize{ get { return 15; } }

		public int OrderItemTextSize{ get { return 12; } }
		public int OrderItemModifierTextSize{ get { return 8; } }

		public int ButtonFontSize{get{return 12;}}

		public int CategoryTextSize{get{return 14;}}
		public int CategoryTopTextSize{get{ return 18;}}

		public int MaxTabNameLength {get{ return 16;}}
		#endregion

		int _ordersScreenCommandBarHeight;
		public int OrdersScreenCommandBarHeight{get{return _ordersScreenCommandBarHeight;}}

		public int CommandButtonWidth{get{ return 210; }}

		int _topRowOnOrderHeight;
		public int TopRowOnOrderHeight{ get { return _topRowOnOrderHeight; } }

		int _loginButtonWidth;
		public int LoginButtonWidth{ get { return _loginButtonWidth; } }
		int _loginButtonHeight;
		public int LoginButtonHeight{ get { return _loginButtonHeight; } }

		public int ClockOutButtonWidth {
			get {
				return _loginButtonWidth;
			}
		}

		public int ClockOutButtonHeight {
			get {
				return _loginButtonHeight;
			}
		}

		public Color ClockOutButtonBackground {
			get {
				return Red;
			}
		}

		public int EmployeeOptionsColumnWidth {
			get { return _loginButtonWidth;}
		}

		int _addTabConfirmButtonWidth;
		public int AddTabConfirmButtonWidth{ get { return _addTabConfirmButtonWidth; } }

		public int AddTabButtonWidth {
			get {				return _loginButtonWidth;			}
		}

		public int AddTabButtonHeight {
			get {				return _loginButtonHeight;			}
		}

		/// <summary>
		/// Color of any items in the main category
		/// </summary>
		/// <value>The color of the main category.</value>
		public Color MainCategoryColor{ get{return Green;}}

		/// <summary>
		/// Color of any items in the sub category
		/// </summary>
		/// <value>The color of the sub category.</value>
		public Color SubCategoryColor{ get{ return CategoryBlack;} }

		public Color AddTabButtonColor {
			get {return Green;}
		}

		public int EmpOptionsButtonWidth {
			get {
				return _loginButtonWidth;
			}
		}

		public int EmpOptionsButtonHeight {
			get {
				return _loginButtonHeight;
			}
		}

		public Color EmpOptionsButtonColor {
			get {
				return Blue;
			}
		}

		public Color DefaultTextColor{ get { return White; } }
		public Color DisabledTextColor{get{return Grey;}}

		int _employeeButtonWidth;
		public int EmployeeButtonWidth{ get { return _employeeButtonWidth; } }

		int _employeeButtonHeight;
		public int EmployeeButtonHeight{ get { return _employeeButtonHeight; } }

		int _tabButtonWidth;
		public int TabButtonWidth{ get { return _tabButtonWidth;} }

		int _tabButtonHeight;
		public int TabButtonHeight{ get { return _tabButtonHeight; } }

		int _menuItemButtonWidth;
		public int MenuItemButtonWidth{ get { return _menuItemButtonWidth; } }

		int _menuItemButtonHeight;
		public int MenuItemButtonHeight{ get { return _menuItemButtonHeight; } }

		int _ordersScreenCenterColumnWidth;
		public int OrdersScreenCenterColumnWidth{get{return _ordersScreenCenterColumnWidth;}}

		int _menuItemButtonPadding;
		public int MenuItemButtonPadding{ get{ return _menuItemButtonPadding;} }

		public int CategoryMenuWidth {
			get {
				return _categoryButtonWidth;
			}
		}
			
		public int CategoryNavButtonWidth{get{ return 125;}}

		int _categoryMenuHeight;
		public int CategoryMenuHeight {
			get {
				return _categoryMenuHeight;
			}
		}

		public Color CategoryRowTextColor{ get{ return White;} }
		int _categoryButtonWidth;
		public int CategoryButtonWidth{ get { return _categoryButtonWidth; } }

		int _categoryButtonHeight;
		public int CategoryButtonHeight{ get { return _categoryButtonHeight; } }

		int _categoryColumnHeight;
		public int CategoryColumnHeight{ get { return _categoryColumnHeight; } }

		int _orderScreenCenterScrollerHeight;
		public int OrderScreenCenterScrollerHeight{ get { return _orderScreenCenterScrollerHeight; } }

		int _paymentsPaymentSpecificAreaScrollerHeight;
		public int PaymentsPaymentSpecificAreaScrollerHeight{ get { return _paymentsPaymentSpecificAreaScrollerHeight; } }

		int _paymentsPaymentListScrollerHeight;
		public int PaymentsPaymentListScrollerHeight{ get { return _paymentsPaymentListScrollerHeight; } }

		int _middleRowHeight;
		public int OrderMiddleRowHeight{ get{return _middleRowHeight;} }

		int _orderCommandDividerHeight;
		public int OrderCommandDividerHeight {
			get {
				return _orderCommandDividerHeight;
			}
		}

		int _modItemWidth;
		public int ModifierItemWidth {
			get {
				return _modItemWidth;
			}
		}
			

		public int ModifierMemoWidth {
			get {
				return _menuItemButtonWidth * 4;
			}
		}

		public Color ModifierTextColor {
			get { 
				return Purple;
			}
		}

		public int OrdersScreenModifierTitleWidth {
			get {
				return 250;
			}
		}

		public Color MiseItemColor {
			get{ return Green; }
		}

		public int ModifiersTopTextSize{get{return 16;}}


		int _orderItemPanelWidth;
		public int OrderItemPanelWidth{ get { return _orderItemPanelWidth;} }

		public Color UnknownCategoryColor{ get { return Black; } }
		public Color WindowBackground{ get { return Grey; } }

		public int ButtonPadding{ get { return 2; } }

		public Typeface Typeface{ get { return Typeface.Create("sans", TypefaceStyle.Normal); } }
		public TypefaceStyle TypefaceStyle{ get { return TypefaceStyle.Normal; } }

		public Color LoginButtonBackground{get{return Grey;}}


		public Color EmployeeButtonBackground{get{return Black;}}
		public Color EmployeeButtonTextColor{ get { return Color.White; } }
	
		public int NumTabColumns{ get { return 4; } }

		public int NumberOfHotItems{ get { return 10; } }
		public int NumMenuItemColumns{ get { return 3; } }
		public int MaxNumMiseItems{ get { return 12; } }

		public Color HotItemColor{ get { return Orange; } }

		/// <summary>
		/// Color for modifiers that need to be hit, but don't have a default
		/// </summary>
		/// <value>The color of the required modifier button.</value>
		public Color RequiredModifierButtonColor{ get { return ModifierGrey; } }
		public Color PossibleModifierButtonColor{ get { return ModifierGrey; } }
		public Color CommitModifiersButtonColor{ get { return Green; } }


		public Color CancelButtonColor{ get { return Red; } }

		public Color CategoryButtonColor{ get { return Green; } }
		public Color CategoryTextColor{ get { return White; } }


		public Color CloseOrderButtonColor{ get { return Purple; } }
		public Color ExtrasColor{ get { return Yellow; } }

		public Color OKButtonColor{get{return Green;}}

		/// <summary>
		/// The height of the Tab check button on the order screen
		/// </summary>
		/// <value>The height of the tab check button.</value>
		public Color SendCheckButtonColor{ get { return Blue; } }

		public Color NoSaleButtonColor{ get { return Yellow; } }

		public Color SelectedButtonColor{ get { return White; } }
		public Color SelectedButtonTextColor{ get { return Color.Black; } }


		public Color OrderItemListBackgroundColor{get{return LightGray;}}
		public Color OrderItemTextColor{ get { return Grey; } }
		public Color OrderItemTextColorUnsent{ get { return Blue; } }
		public Color OrderItemTextColorSent{get{return Grey;}}


		public Color VoidItemBackgroundColor{ get { return Yellow; } }

		public Color CategoryHomeColor {get{return CategoryBlack;}}
		public Color CategoryUpColor{get{return Blue;}}


		public int CashCloseTotalTextSize { get {return 32;}}
		public int NumberOfCashButtons{ get { return 6; } }

		#region Payment
		int _paymentScreenMiddleColumnWidth;
		public int PaymentsScreenMiddleColumnWidth {
			get {
				return _paymentScreenMiddleColumnWidth;
			}
		}

		int _paymentScreenUpperMiddleHeight;
		public int PaymentsScreenUpperMiddleHeight{ get{ return _paymentScreenUpperMiddleHeight;} }

		public int PaymentsScreenRightColumnWidth {
			get {
				return _categoryButtonWidth;
			}
		}

		public int ReopenButtonHeight{get{ return _cashButtonHeight;}}
		public Color ReopenButtonColor{ get { return Yellow; } }

		int _cashButtonHeight;
		public int CashButtonHeight{ get { return _cashButtonHeight; } }
		public Color CashButtonColor{ get{return Green;} }

		public int CreditButtonHeight{ get { return _cashButtonHeight; } }
		public Color CreditButtonColor{get{return Blue;}}

		public int AddGratuityHeight{get{return _cashButtonHeight;}}
		public Color AddGratuityColor{get{return Pink;}}

		public int PaymentsAmountTextSize{ get { return 14; } }

		public int PaymentsCancelButtonHeight{get{return _ordersScreenCommandBarHeight;}}
		public int PaymentsSendButtonHeight{get{return _ordersScreenCommandBarHeight;}}

		public int PayCheckHeight{ get { return _cashButtonHeight; } }
		public Color PayCheckColor{get{return Green;}}

		public Color PayPercentageColor{get{ return Blue;}}

		public int NumberCashPaymentChoices{get{return 12;}}

		public Color CreditCardTextColor{get{return White;}}
		public int CreditCardTextSize{get{return 16;}}

		int _paymentSpaceHeight;
		public int PaymentSpaceHeight{get{return _paymentSpaceHeight; }}
		public int PaymentTextSize{get{ return 16;}}

		/// <summary>
		/// What color the text showing the payment type is
		/// </summary>
		/// <value>The color of the payment type.</value>
		public  Color GetPaymentTypeColor (PaymentType type){
			switch(type){
			case PaymentType.CompAmount:
				return CompColor;
			case PaymentType.Cash:
				return CashButtonColor;
			default:
				return DefaultTextColor;
			}
		}

		public Color PaymentsSentButtonColor{get{return SendCheckButtonColor;}}

		public Color AddTipColor{ get{return Green;}}
		#endregion

		/// <summary>
		/// Color given to all comp items.  
		/// </summary>
		/// <value>The color of the comp.</value>
		public Color CompColor {get{return Blue;}}

		public int CompItemButtonWidth {get{ return _menuItemButtonWidth; }}

		public int CompItemButtonHeight { get { return _menuItemButtonHeight; } }

		public int PaymentTypeWidth{get{ return PaymentItemButtonWidth;}}
		public int PaymentAmountWidth{get{ return PaymentItemButtonWidth;}}
		public int PaymentItemButtonWidth{get{ return _menuItemButtonWidth;}}

		int _paymentAddCreditCardIndent;
		public int PaymentAddCreditCardIndent{get{ return _paymentAddCreditCardIndent;}}

		public Color DiscountColor {
			get {
				return Green;
			}
		}

		public Color GratuityColor {
			get {
				return Pink;
			}
		}
	}
}

