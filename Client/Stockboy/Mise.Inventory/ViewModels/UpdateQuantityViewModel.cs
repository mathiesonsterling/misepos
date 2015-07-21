using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using System.Dynamic;
using Xamarin.Forms;
using Mise.Core.ValueItems;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public class UpdateQuantityViewModel : BaseViewModel
	{
		int _originalQuantity;
		decimal _originalTotalPrice;

		bool justAdjustedUnit = false;
		bool justAdjustedTotal = false;
		public UpdateQuantityViewModel(IAppNavigation appNavigation, ILogger logger) : base(appNavigation, logger){

			PropertyChanged += (sender, e) => {
				if(e.PropertyName == "UnitPrice"){
					var calcTotal = UnitPrice * CurrentQuantity;
					calcTotal = Math.Round(calcTotal, 2);
					if(calcTotal != TotalPrice){
						TotalPrice = calcTotal;
					}
				}

				if(e.PropertyName == "TotalPrice"){
					if(justAdjustedUnit){
						justAdjustedUnit = false;
						return;
					}

					//adjust the unit
					justAdjustedTotal = true;
					if(CurrentQuantity > 0)
					{
						var unitPrice = TotalPrice / CurrentQuantity;
						UnitPrice = Math.Round(unitPrice);
					} else{
						UnitPrice = 0;
					}
				}

				if(e.PropertyName == "CurrentQuantity"){
				}
				if(e.PropertyName == "CurrentQuantity" || e.PropertyName == "TotalPrice"){
					if(DoPrices){
						if(CurrentQuantity > 0){
							var calcPrice = Math.Round(TotalPrice / CurrentQuantity, 2);
							if(calcPrice != UnitPrice){
								UnitPrice = calcPrice;
							} 
						} else {
							UnitPrice = 0;
						}
						IsUpdateEnabled = (CurrentQuantity != _originalQuantity 
							|| TotalPrice != _originalTotalPrice)
						    && TotalPrice > 0;
					} else {
						IsUpdateEnabled = CurrentQuantity != _originalQuantity;
					}
				}
			};
		}

		#region Callbacks
		public Action<int, decimal> OnUpdatedCallback{ get; set;}
		public Action ZeroedOutCallback{ get; set;}
		#endregion

		public int CurrentQuantity{get{return GetValue<int>();}set{ SetValue (value); }}
		public string ItemName{get{return GetValue<string> ();}set{SetValue (value);}}
		public bool IsUpdateEnabled{ get; set;}

		public decimal TotalPrice{get{return GetValue<decimal> ();}set{ SetValue (value); }}
		public decimal UnitPrice{get{return GetValue<decimal> ();}set{ SetValue (value); }}

		public bool DoPrices{get{return GetValue<bool> ();}set{ SetValue (value); }}
		public bool ShowZeroOut{get{return DoPrices && ZeroedOutCallback != null;}}
		public string Title{ get { return GetValue<string> (); } set { SetValue (value); } }

		#region Commands
		public ICommand CancelCommand{get{return new SimpleCommand (Cancel);}}

		public ICommand UpdateQuantityCommand{get{return new SimpleCommand (UpdateQuantity, () => IsUpdateEnabled);}}

		public ICommand ZeroOutCommand{get{return new SimpleCommand (ZeroOut);}}
		#endregion

		public override Task OnAppearing()
		{
			return Task.FromResult(false);
		}
		/// <summary>
		/// Sets the quantity for this screen before it's displayed
		/// </summary>
		/// <param name="quant">Quant.</param>
		/// <param name = "itemName"></param>
		/// <param name = "price"></param>
		public void SetQuantity(int quant, string itemName, Money price){
			_originalQuantity = quant;
			CurrentQuantity = quant;
			if (DoPrices) {
				TotalPrice = price != null ? price.Dollars : 0;
				_originalTotalPrice = TotalPrice;
			}
			ItemName = itemName;
			IsUpdateEnabled = false;
		}
			
		async void Cancel(){
			await Navigation.CloseUpdateQuantity ();
		}

		async void ZeroOut(){
			try{
				if(ZeroedOutCallback != null){
					ZeroedOutCallback ();
				}

				await Navigation.CloseUpdateQuantity ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void UpdateQuantity(){
			try{
				if(OnUpdatedCallback != null){
					OnUpdatedCallback (CurrentQuantity, DoPrices ? TotalPrice : 0);
				}

				await Navigation.CloseUpdateQuantity ();
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

