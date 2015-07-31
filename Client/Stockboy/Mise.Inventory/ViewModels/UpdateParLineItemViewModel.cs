using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class UpdateParLineItemViewModel : BaseNextViewModel<IPARBeverageLineItem>
	{
		readonly IPARService _parService;
		public UpdateParLineItemViewModel(IAppNavigation appNav, ILogger logger, IPARService parService) 
			: base(appNav, logger){
			_parService = parService;
		}

	    public override async Task OnAppearing()
	    {
	        await base.OnAppearing();

			//do we have a selected item?
			var selected = await _parService.GetCurrentLineItem();
			if (selected != null) {
				SetCurrent (selected);
			}
	        ItemName = CurrentItem != null ? CurrentItem.DisplayName : string.Empty;
			NextItemName = NextItem != null ? NextItem.DisplayName + " >>" : string.Empty;
	        CurrentQuantity = CurrentItem != null ? CurrentItem.Quantity : 0;
	    }

	    #region Fields
		public string ItemName{ get { return GetValue<string> (); } private set { SetValue(value);} }
        public decimal CurrentQuantity {
            get { return GetValue<decimal>(); }
            set{SetValue(value);}
        }
		public string NextItemName{ get { return GetValue<string> (); } private set { SetValue (value); } }

		#endregion

        #region Commands
		public ICommand UpdateQuantityCommand { 
			get { return new Command(UpdateQuantity, () => NotProcessing);} 
		}

	    private async void UpdateQuantity()
	    {
	        await _parService.UpdateQuantityOfPARLineItem(CurrentItem, CurrentQuantity);
	        await Navigation.CloseUpdateQuantity();
	    }

	    #endregion

        #region implemented abstract members of BaseNextViewModel

        protected override async Task<IList<IPARBeverageLineItem>> LoadItems ()
		{
		    var currentPar = await _parService.GetCurrentPAR();
			return currentPar.GetBeverageLineItems().OrderBy(li => li.DisplayName).ToList();
		}

		protected override async Task BeforeMove(IPARBeverageLineItem currentItem)
		{
			//update the current one
		    if (CurrentItem != null && CurrentItem.Quantity != CurrentQuantity)
		    {
		        await _parService.UpdateQuantityOfPARLineItem(CurrentItem, CurrentQuantity);
		    }
		}

		protected override async Task AfterMove (IPARBeverageLineItem newItem)
		{
			await _parService.SetCurrentLineItem (newItem);
		    await OnAppearing();
		}

		#endregion
	}
}

