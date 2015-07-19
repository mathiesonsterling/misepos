using System;

namespace Mise.Core.Services
{
    public delegate void OnCashDrawerStatusChanged(bool isOpen);

    /// <summary>
    /// Abstracts the functioning of the cash drawer
    /// </summary>
    public interface ICashDrawerService
    {
		/// <summary>
		/// Command the cash drawer to open
		/// </summary>
        void OpenDrawer();

		/// <summary>
		/// Event that is fired when we close the cash drawer
		/// </summary>
		event EventHandler DrawerClosed;
    }
}
