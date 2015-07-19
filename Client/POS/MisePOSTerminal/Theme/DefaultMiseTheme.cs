using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Forms;

using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
namespace MisePOSTerminal.Theme
{
	public class DefaultMiseTheme : ITheme
	{
		#region ITheme implementation

		private Dictionary<Guid, Color> _employeeAssignedColors = new Dictionary<Guid, Color> ();

		public void LoadColorForEmployee (Guid id, int r, int g, int b)
		{
			//if our emp color exists, see if it's the same
			if (_employeeAssignedColors.ContainsKey (id)) {
				var exColor = _employeeAssignedColors [id];
				if(exColor.R == r && exColor.G == g && exColor.B == b) {
					return;
				}
			} 
				var color = Color.FromRgb (r, g, b);
				_employeeAssignedColors [id] = color;

		}
	
		public Color GetColorForEmployee(Mise.Core.Entities.People.IEmployee emp){
			return GetColorForEmployee (emp.ID, emp.PreferredColorName);
		}

		public Color GetColorForEmployee (Guid empID, string preferredColorName = "")
		{
			if (_employeeAssignedColors.ContainsKey (empID)) {
				return _employeeAssignedColors [empID];
			}

			Color col = Color.Default;
			var random = new Random ();
			if(string.IsNullOrEmpty(preferredColorName) || (_allEmployeeColors.ContainsKey(preferredColorName) == false))
			{
				//just get the next unassigned
				var unassigned = _allEmployeeColors.Values.Where (c => _employeeAssignedColors.Values.Contains (c) == false).ToList();
				if (unassigned.Any ()) {
					var index = random.Next (unassigned.Count ());
					col = unassigned [index];
				} else {
					var index = random.Next (_allEmployeeColors.Count ());
					col = _allEmployeeColors.Values.ToList()[index];
				}	
			}

			//store it
			_employeeAssignedColors.Add (empID, col);
			return col;
		}

		public Color GetColorForCheck (ICheck check)
		{
			if (check.LastTouchedServerID != Guid.Empty) {
				if (_employeeAssignedColors.ContainsKey (check.LastTouchedServerID)) {
					return _employeeAssignedColors [check.LastTouchedServerID];
				} 
			}

			//assign a new color!
			return Color.Default;
		}

		public Color GetColorForPaymentStatus (CheckPaymentStatus status)
		{
			switch (status) {
			case CheckPaymentStatus.Closed:
				return Blue;
			case CheckPaymentStatus.ClosedAndZed:
				return CategoryBlack;
			case CheckPaymentStatus.Closing:
				return Yellow;
			case CheckPaymentStatus.Open:
				return Green;
			case CheckPaymentStatus.PaymentApprovedWithoutTip:
				return Purple;
			case CheckPaymentStatus.PaymentRejected:
				return Red;
			default :
				throw new ArgumentException ("No color for status " + status);
			}
		}

		public IEnumerable<Color> Colors {
			get {
				return _allEmployeeColors.Values;
			}
		}

		public Color TextColor {
			get {
				return Color.White;
			}
		}
			
		public Color SelectedTextColor{get{return Color.Black;}}
		public Color SelectedBackgroundColor{get{return Color.White;}}

		//public int ButtonFontSize{get{ return 24;}}
		public Font ButtonFont{ get { return Font.SystemFontOfSize (24); } }
		public int BorderRadius {
			get {
				return 0;
			}
		}

		public int EmployeeButtonHeight {
			get {
				return 125;
			}
		}
	
		public int CheckButtonHeight{
			get{
				return 125;
			}
		}

		public int NumChecksPerRow { get { return 3; } }

		public Color CategoryColor{get{ return CategoryBlack;}}
		public int CategoryButtonHeight{ get{ return 65;} }

		public int OrderItemFontSize {
			get {
				return 30;
			}
		}

		public Color OrderItemFontColor{ get{ return Black;} }
		public Color OrderDividerLineColor{ get{ return OrderDivider;}}
		public int ModifierFontSize {
			get {
				return 24;
			}
		}


		public Color ModifierFontColor {
			get {
				return Black;
			}
		}


		public int MenuItemButtonPadding {
			get {
				return 23;
			}
		}

		public int MenuItemButtonWidth{get{return 212;}}

		public int MenuItemButtonFontSize { get { return 24; } }

		public Color HotItemColor {
			get {
				return Orange;
			}
		}


		public Color MiseItemColor {
			get {
				return Green;
			}
		}


		public Color SubCatItemColor {
			get {
				return CategoryBlack;
			}
		}


		public Color MenuItemColor {
			get {
				return Green;
			}
		}

		#endregion

		#region Colors
		static readonly Color Green = Color.FromRgb(63, 218, 107);
		static readonly Color Orange = Color.FromRgb (225, 100, 58);
		static readonly Color Red = Color.FromRgb (212, 50, 81);
		static readonly Color Blue = Color.FromRgb (65, 169, 242);
		static readonly Color Yellow = Color.FromRgb (226, 208, 56);
		static readonly Color Purple = Color.FromRgb (157, 70, 207);


		static readonly Color Black = Color.FromRgb(35,35,35);
		static readonly Color CategoryBlack = Color.FromRgb (51, 51, 51);


		static readonly Color OrderDivider = Color.FromRgb(210, 210, 210);

	    readonly IDictionary<string, Color> _allEmployeeColors = new Dictionary<string, Color>{
			{"orange",Orange},
			{"yellow",Yellow},
			{"purple",Purple},
			{"green",Green},
		};
		#endregion
	}
}

