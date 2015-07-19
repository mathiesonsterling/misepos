using System;
using System.Threading.Tasks;
using Mise.Inventory.Services;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public class ReportsViewModel : BaseViewModel
	{
		public ReportsViewModel(IAppNavigation navigation, ILogger logger) : base(navigation, logger)
		{
		}

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }
	}
}

