﻿using System.Threading.Tasks;

using Xamarin.Forms;

namespace Mise.POSTerminal.Services
{
	public interface INavigationService :INavigation
	{
		Task<bool> DisplayAlert(string title, string message, string accept, string cancel = null);
	}
}
