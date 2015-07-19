using System;
using System.Threading;
using System.Threading.Tasks;

using Mise.Core.Services;

namespace Mise.POSTerminal.Android.Services
{
	public class CashDrawerService : ICashDrawerService
	{
		public void OpenDrawer()
		{
			//fire a task to close the drawer in 10 seconds
			Task.Factory.StartNew(() => {
				Thread.Sleep(10000);
				// more code
				if (DrawerClosed != null) {
					DrawerClosed(this, new EventArgs());
				}
			});
		}

		public event EventHandler DrawerClosed;
	}
}


