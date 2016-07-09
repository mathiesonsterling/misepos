using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

using Mise.Inventory.Services;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
    public class UpdateReceivingOrderLineItemViewModel : BaseNextViewModel<IReceivingOrderLineItem>
    {
        private readonly IReceivingOrderService _receivingOrderService;
        private bool _updatedTotal;
        private bool _updatedUnit;
        public UpdateReceivingOrderLineItemViewModel(IAppNavigation navigationService, ILogger logger, IReceivingOrderService receivingOrderService) 
            : base(navigationService, logger)
        {
            _receivingOrderService = receivingOrderService;
			IsUpdateEnabled = false;
              
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "UnitPrice")
                {
                    if (_updatedTotal)
                    {
                        _updatedTotal = false;
                        return;
                    }
                    _updatedUnit = true;
                    TotalPrice = Math.Round(UnitPrice*CurrentQuantity, 2);
                    CanUpdate();
                }

                if (args.PropertyName == "TotalPrice")
                {
                    if (_updatedUnit)
                    {
                        _updatedUnit = false;
                        return;
                    }
                    _updatedTotal = true;
					if(CurrentQuantity > 0){
                    	UnitPrice = Math.Round(TotalPrice/CurrentQuantity, 2);
					} else {
						UnitPrice = 0;
					}
                    CanUpdate();
                }

                if (args.PropertyName == "CurrentQuantity")
                {
					//update our prices - total will trigger the unit
					TotalPrice = Math.Round(UnitPrice * CurrentQuantity, 2);
                    CanUpdate();
                }
            };
        }

        public override async Task OnAppearing()
        {
            await base.OnAppearing();

			var selItem = await _receivingOrderService.GetCurrentLineItem ();
			if (selItem != null) {
				SetCurrent (selItem);
			}

            CurrentQuantity = (int)(CurrentItem?.Quantity ?? 0);
            ItemName = CurrentItem != null ? CurrentItem.DisplayName : string.Empty;
            TotalPrice = CurrentItem != null && CurrentItem.LineItemPrice != null ? CurrentItem.LineItemPrice.Dollars : 0;
            UnitPrice = CurrentItem != null && CurrentItem.UnitPrice != null ? CurrentItem.UnitPrice.Dollars : 0;
            IsUpdateEnabled = false;

			NextItemName = NextItem != null ? NextItem.DisplayName + " >>" : string.Empty;
			UpdateAndMoveToNext = IsUpdateEnabled && NextItem != null;
        }

        #region Fields
        public int CurrentQuantity { get { return GetValue<int>(); } set { SetValue(value); } }
        public string ItemName { get { return GetValue<string>(); } set { SetValue(value); } }
		public bool IsUpdateEnabled { get { return GetValue<bool> (); } set { SetValue (value); } }
		public bool UpdateAndMoveToNext{ get { return GetValue<bool> (); } set { SetValue (value); } }
		public string NextItemName{ get { return GetValue<string> (); } private set { SetValue (value); } }
        public decimal TotalPrice { get { return GetValue<decimal>(); } set { SetValue(value); } }
        public decimal UnitPrice { get { return GetValue<decimal>(); } set { SetValue(value); } }

        #endregion

        #region Commands
        public ICommand UpdateQuantityCommand { get { return new Command(UpdateQuantity, () => IsUpdateEnabled); } }

        private async void UpdateQuantity()
        {
			CanUpdate ();
            if (IsUpdateEnabled == false)
            {
                return;
            }
            var newQuant = CurrentQuantity;
            var newPrice = new Money(TotalPrice);
            Processing = true;
            await _receivingOrderService.UpdateQuantityOfLineItem(CurrentItem, newQuant, newPrice);
            Processing = false;
			await Navigation.CloseUpdateQuantity ();
        }

        private void CanUpdate()
        {
			var res = true;
			if (CurrentItem == null) {
				res = false;
				NextItemName = string.Empty;
				UpdateAndMoveToNext = false;
				IsUpdateEnabled = false;
				return;
			}
            var oldQuant = CurrentItem.Quantity;
            var oldTotal = CurrentItem.LineItemPrice;
			if (oldTotal == null) {
				res = TotalPrice != 0;
			}

			if (res) {
				var newQuant = CurrentQuantity;
				var newPrice = new Money (TotalPrice);

				res = newQuant > 0 && (oldQuant != newQuant || (oldTotal.Equals (newPrice) == false));
			}
				
			UpdateAndMoveToNext = res && NextItem != null;
			IsUpdateEnabled = res;
        }

		public ICommand ZeroOutCommand { get { return new Command(ZeroOut, () => NotProcessing); } }

        private async void ZeroOut()
        {
            Processing = true;
            await _receivingOrderService.ZeroOutLineItem(CurrentItem);
            Processing = false;
			await Navigation.CloseUpdateQuantity ();
        }

        #endregion

        #region BaseNextViewModel members
        protected override async Task<IList<IReceivingOrderLineItem>> LoadItems()
        {
            var current = await _receivingOrderService.GetCurrentReceivingOrder();
            return current.GetBeverageLineItems().ToList();
        }

        protected override async Task BeforeMove(IReceivingOrderLineItem currentItem)
        {
            //update the service with our new quantity
            var totalPrice = new Money(TotalPrice);
            await _receivingOrderService.UpdateQuantityOfLineItem(CurrentItem, CurrentQuantity, totalPrice);
        }

        protected override async Task AfterMove(IReceivingOrderLineItem newItem)
        {
			CurrentQuantity = (int)newItem.Quantity;
			TotalPrice = newItem.LineItemPrice.Dollars;
			UnitPrice = newItem.UnitPrice.Dollars;
            await OnAppearing();
        }
        #endregion
    }
}
