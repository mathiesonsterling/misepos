using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mise.Inventory.Services;

using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class UpdateParQuantityViewModel : BaseNextViewModel<IParBeverageLineItem>
	{
		private readonly IPARService _parService;
		public UpdateParQuantityViewModel(IAppNavigation navigationService, ILogger logger, IPARService parService) 
			: base(navigationService, logger){
			_parService = parService;

		}

		public override Task OnAppearing ()
		{
			CurrentQuantity = CurrentItem?.Quantity ?? 0;
			ItemName = CurrentItem != null ? CurrentItem.DisplayName : string.Empty;
		    return Task.FromResult(false);
		}

		#region Fields
		public decimal CurrentQuantity{get{return GetValue<decimal>();}set{ SetValue (value); }}
		public string ItemName{get{return GetValue<string> ();}set{SetValue (value);}}
		#endregion

		#region implemented abstract members of BaseNextViewModel
		protected override async Task<IList<IParBeverageLineItem>> LoadItems ()
		{
			var currentPar = await _parService.GetCurrentPAR ();
			if (currentPar != null) {
				return currentPar.GetBeverageLineItems ().ToList ();
			}

			throw new InvalidOperationException ("No current par to update");
		}

	    protected override Task BeforeMove (IParBeverageLineItem currentItem)
		{
			//update the par
			return UpdateQuantityWithoutMoving();
		}

		protected override async Task AfterMove (IParBeverageLineItem newItem)
		{
			await OnAppearing();
		}

		#endregion

		public ICommand UpdateQuantityCommand{get{return new Command (
				UpdateQuantity, 
				() => CurrentItem != null && CurrentQuantity != CurrentItem.Quantity);}}

		private async void UpdateQuantity(){
			await UpdateQuantityWithoutMoving ();
			await Navigation.CloseUpdateQuantity ();
		}

		private async Task UpdateQuantityWithoutMoving(){
			if (CurrentItem.Quantity != CurrentQuantity) {
				await _parService.UpdateQuantityOfPARLineItem (CurrentItem, CurrentQuantity);
			}

		}
	}
}

