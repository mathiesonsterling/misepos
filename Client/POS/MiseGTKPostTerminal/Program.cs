using System;
using Gtk;

namespace MiseGTKPostTerminal
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();

            var theme = new DefaultTheme();
		    Settings.Default.ThemeName = theme.GTKThemeValue;
			var win = new MainWindow (theme);

            //win.Fullscreen();
			win.ShowAll ();
			Application.Run ();
		}
	}
}
