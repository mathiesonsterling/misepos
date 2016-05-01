using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Inventory.ViewModels;

namespace StockboyForms.ViewModels
{
	public class UpdateParQuantityViewModel : BaseNextViewModel<IParBeverageLineItem>
	{
		private readonly IPARService _parService;
		public UpdateParQuantityViewModel(IAppNavigation navigationService, ILogger logger, IPARService parService) 
			: base(navigationService, logger){
			_parService = parService;

		}

		public override async Task OnAppearing ()
		{
			CurrentQuantity = CurrentItem != null ? CurrentItem.Quantity : 0;
			ItemName = CurrentItem != null ? CurrentItem.DisplayName : string.Empty;

		}

		#region Fields
		public decimal CurrentQuantity{get{return GetValue<int>();}set{ SetValue (value); }}
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

		protected override Task<IParBeverageLineItem> GetSelectedItem ()
		{
			return _parService.GetLineItemToMeasure ();
		}

		#endregion

		public ICommand UpdateQuantityCommand{get{return new SimpleCommand (
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

