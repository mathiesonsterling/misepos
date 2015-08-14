using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Inventory.Services;

using System.Windows.Input;
using Mise.Inventory.MVVM;
namespace Mise.Inventory.ViewModels
{
	public class UpdateParQuantityViewModel : BaseNextViewModel<IPARBeverageLineItem>
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
		public int CurrentQuantity{get{return GetValue<int>();}set{ SetValue (value); }}
		public string ItemName{get{return GetValue<string> ();}set{SetValue (value);}}
		#endregion

		#region implemented abstract members of BaseNextViewModel
		protected override async Task<IList<IPARBeverageLineItem>> LoadItems ()
		{
			var currentPar = await _parService.GetCurrentPAR ();
			if (currentPar != null) {
				return currentPar.GetBeverageLineItems ().ToList ();
			}

			throw new InvalidOperationException ("No current par to update");
		}

		protected override Task BeforeMoveNext (IPARBeverageLineItem currentItem)
		{
			//update the par
			return UpdateQuantityWithoutMoving();
		}

		protected override async Task AfterMoveNext (IPARBeverageLineItem newItem)
		{
			await OnAppearing();
		}

		protected override Task<IPARBeverageLineItem> GetSelectedItem ()
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

