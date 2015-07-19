using Mise.Core.Entities;

namespace Mise.POSTerminal.Theme
{
	public static class ThemeFactory
	{
		public static ITheme GetTheme(IMiseTerminalDevice device)
		{
			return new DefaultMiseTheme();
		}
	}
}

