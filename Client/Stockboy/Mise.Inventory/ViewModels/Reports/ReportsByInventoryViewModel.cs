using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using Mise.Inventory.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Reports;
using Xamarin.Forms;
using Mise.Inventory.Services.Implementation;
using Mise.Core.ValueItems.Inventory;

namespace Mise.Inventory.ViewModels.Reports
{
	public class ReportsByInventoryViewModel : ReportsViewModel
	{
		public ReportsByInventoryViewModel (IAppNavigation navi, ILogger logger, IInventoryService inventoryService,
		IReportsService reportsService, ILoginService loginService) 
			: base(navi, logger, reportsService, inventoryService, loginService)
		{
		}

		#region implemented abstract members of BaseViewModel

		public override async Task OnAppearing ()
		{
			await base.OnAppearing ();
			StartInventory = null;
			EndInventory = null;

			//load the inventories into the possibles
			CompletedInventories = await _inventoryService.GetCompletedInventoriesForCurrentRestaurant (null, null);
		}

		#endregion

		public IEnumerable<IInventory> CompletedInventories {
			get{ return GetValue<IEnumerable<IInventory>> (); }
			private set{ SetValue (value); }
		}

		public IInventory StartInventory{get{return GetValue<IInventory>();}
			set{ 
				StartDate = value.DateCompleted.Value.DateTime;
				SetValue(value);
			}
		}
		public IInventory EndInventory{
			get{
				return GetValue<IInventory> ();
			}
			set{ 
				SetValue (value); }}

	}
}

