using System.Threading.Tasks;
using Xamarin.Forms;

namespace Mise.Inventory.Services
{
	public interface INavigationService : INavigation
	{
		Page CurrentPage{ get; set; }

		Task DisplayAlert(
			string title,
			string message,
			string accept = "OK"
		);

		Task<bool> AskUser(string title, string message, string accept="OK", string cancel="Cancel");
	}
}