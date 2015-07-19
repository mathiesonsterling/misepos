using System;

using Mise.Core.Entities;

namespace MisePOSTerminal.Theme
{
	public static class ThemeFactory
	{
		public static ITheme GetTheme(IMiseTerminalDevice device){
			return new DefaultMiseTheme ();
		}
	}
}

