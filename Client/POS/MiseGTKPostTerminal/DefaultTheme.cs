using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gdk;

namespace MiseGTKPostTerminal
{
    /// <summary>
    /// Class to determine our colors and widths
    /// </summary>
    public class DefaultTheme
    {
        private static readonly Color White = new Color(0, 255, 255);
        private static readonly Color Black = new Color(0, 0, 0);
        private static readonly Color Purpleish = new Color(75, 0, 130);
        private static readonly Color Aqua = new Color(127, 255, 212);
        private static readonly Color Orange = new Color(255, 165, 0);
		private static readonly Color Green = new Color (0, 255, 0);
		private static readonly Color Yellow = new Color (255, 255, 0);
		private static readonly Color Red = new Color (255, 0, 0);

		private static readonly TimeSpan _cashDrawerDisplayTime = new TimeSpan (0, 0, 5);
		private static readonly TimeSpan _timeTillScreenSaver = new TimeSpan(0, 2, 0);

        public uint EmployeeBorderOnTabScreenWidth { get { return 5; } }
	
        public uint NumberOfTabColumns { get { return 4; } }
        public uint PaddingBetweenTabs { get { return 3; } }

        public string GTKThemeValue { get { return "Clearlooks"; } }



        public Color WindowBackground{get { return Black; }}

        public Color ButtonTextColor { get { return White; } }

		public Color OrderItemColor{ get { return White; } }

		public int WindowWidth{ get { return 1024; } }

		public int WindowHeight{ get { return 768; } }

		public int DialogWidth{ get { return 768; } }
		public int DialogHeight{ get { return 512; } }

		#region Buttons
		public Color SelectedEmployeeButtonColor { get { return Purpleish; } }

		public Color AdminItemButtonColor{get{return Aqua;}}

		public Color CategoryButtonColor{get { return Orange; }}

		public Color CashButtonColor{ get { return Green; } }
		public int MenuItemButtonWidth{ get { return 100; } }
		public int MenuItemButtonHeight{ get { return 75; } }

		public Color PaymentPendingColor{ get { return Yellow; } }
		public Color PaymentRejectedColor{ get { return Red; } }
		#endregion

		#region Order Panel
		public int OrderScreenOrderPanelWidth{get{return 300;}}
		public int OrderScreenMenuItemsPanelWidth{ get { return 550; } }

		public int ButtonPadding{ get { return 10; } }
		#endregion

		public TimeSpan DisplayCashDrawerTime{ get { return _cashDrawerDisplayTime; } }
		public TimeSpan TimeTillScreenSaver{ get { return _timeTillScreenSaver; } }
    }
}
