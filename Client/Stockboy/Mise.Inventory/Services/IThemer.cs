using Xamarin.Forms;

namespace Mise.Inventory.Services
{
	public interface IThemer
	{
		Color AccentColor { get; }
	}

	public class DefaultThemer : IThemer
	{
		public DefaultThemer()
		{
			AccentColor = Color.Accent;
		}

		public Color AccentColor { get; private set; }
	}
}