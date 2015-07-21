using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Inventory;
using System.Windows.Input;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using Mise.Core.Services;
using Mise.Core.ValueItems.Inventory;


namespace Mise.Inventory.ViewModels
{
	public class InventoryVisuallyMeasureBottleViewModel : BaseNextViewModel<IInventoryBeverageLineItem>
	{

		readonly IInventoryService _inventoryService;

		public InventoryVisuallyMeasureBottleViewModel(IAppNavigation navi,
			IInventoryService inventoryService, 
			ILogger logger) : base(navi, logger){

			_inventoryService = inventoryService;

			SetCurrentLineItem();

			PartialAmounts = new List<decimal>();

			PropertyChanged += (sender, e) => {
				if (e.PropertyName == "CurrentPartial" || e.PropertyName == "NumFullBottles") {
					UpdateTotal ();
					DisplayTotal = Total + CurrentPartial;
					AddPartialEnabled = CurrentPartial > 0;
				}

			};
		}

        /// <summary>
        /// Fired when screen launched, or we move items
        /// </summary>
        public override async Task OnAppearing()
        {
			await base.OnAppearing ();
            //zero out every time
            PartialTotal = 0;
            PartialAmounts = new List<decimal>();
            CurrentPartial = 0;
            NumFullBottles = 0;
			DisplayTotal = 0;
            if (ResetMarkers != null)
            {
                ResetMarkers();
            }
            AddPartialEnabled = false;
            UpdateTotal();
            SetCurrentLineItem();

			DisplayName = CurrentItem.DisplayName;
			NextItemName = NextItem != null ? NextItem.DisplayName : string.Empty;
        }

		public LiquidContainerShape Shape{
			get { 
				if (CurrentItem != null) {
					return CurrentItem.Shape;
				}

				return LiquidContainerShape.DefaultBottleShape;
			}
		}
		public string DisplayName{ get { return GetValue<string> (); } private set { SetValue (value); } }

		public string NextItemName{ get { return GetValue<string> (); } private set { SetValue (value); } }

		public decimal Total{get{ return GetValue<decimal> (); }private set{ SetValue (value); }}
		public decimal DisplayTotal{ get{ return GetValue<decimal> (); }private set{SetValue (value);} }

		public int NumFullBottles{	get { return GetValue<int>(); }
			set { SetValue(value); }}

		public decimal CurrentPartial{get{return GetValue<decimal>();} set{SetValue(value);}}

		public bool AddPartialEnabled{ get { return GetValue<bool> (); } private set { SetValue (value); } }

		public decimal PartialTotal{
			get{return GetValue<decimal>();}
			set{ SetValue(value); }
		}

		public List<decimal> PartialAmounts{
			get{ return GetValue<List<decimal>> (); }
			set{ SetValue (value); }
		}

		public Action ResetMarkers{ get; set; }


		void UpdateTotal(){
			Total = NumFullBottles + PartialTotal;
		}
			

	    protected override async Task<IList<IInventoryBeverageLineItem>> LoadItems()
	    {
            return (await _inventoryService.GetLineItemsForCurrentSection())
                .OrderBy(li => li.InventoryPosition).ToList();
	    }

	    #region Commands
		public ICommand MeasureCommand{get{ return new SimpleCommand (MeasureEv);
			}}

		public ICommand AddPartialCommand{get{ return new SimpleCommand (AddPartial, () => AddPartialEnabled);}}

		public ICommand CancelCommand{get{return new SimpleCommand (Cancel);}}

		#endregion

	    protected override async Task BeforeMoveNext(IInventoryBeverageLineItem currentItem)
	    {
	        await Measure();
	    }

	    protected override async Task AfterMoveNext(IInventoryBeverageLineItem newItem)
	    {
	        await _inventoryService.MarkLineItemForMeasurement(newItem);
	        await OnAppearing();
	    }

		async void MeasureEv(){
			await Measure();
		}

		async Task Measure(){
			try{
				//if we have partials, add em
				AddPartial ();
				await _inventoryService.MeasureCurrentLineItemVisually (NumFullBottles, PartialAmounts);
				NumFullBottles = 0;

				//we have to load here, otherwise the values won't update
				await Navigation.CloseInventoryVisuallyMeasureItem ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		public void Empty(){
			CurrentPartial = 0;
			if (ResetMarkers != null) {
				ResetMarkers ();
			}
		}

		void AddPartial(){
			try{
				//see how many of our markers are set
				if (CurrentPartial > 0) {
					PartialAmounts.Add (CurrentPartial);
					PartialTotal += CurrentPartial;
				}
				Empty();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void Cancel(){
			try{
				PartialAmounts = new List<decimal> ();
				NumFullBottles = 0;
				PartialTotal = 0;
				CurrentPartial = 0;
				await Navigation.CloseInventoryVisuallyMeasureItem ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void SetCurrentLineItem ()
		{
			try{
				//load the line item here, or assume it's loaded elsewhere?
				var item = await _inventoryService.GetLineItemToMeasure ();
			    if (item == null)
			    {
			        await Navigation.CloseInventoryVisuallyMeasureItem();
			    }
			    else
			    {
			        SetCurrent(item);
			    }
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

