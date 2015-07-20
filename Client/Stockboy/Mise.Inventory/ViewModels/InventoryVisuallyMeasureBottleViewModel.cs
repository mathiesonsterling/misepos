using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Mise.Inventory.ViewModels;
using Mise.Core.Entities.Inventory;
using System.Windows.Input;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using Xamarin.Forms;
using Mise.Core.Services;
using System.Runtime.InteropServices;


using System.Runtime.InteropServices;
using Mise.Core.ValueItems;


using System.Runtime.InteropServices;


namespace Mise.Inventory.ViewModels
{
	public class InventoryVisuallyMeasureBottleViewModel : BaseViewModel
	{

		readonly IInventoryService _inventoryService;
		readonly ILoginService _loginService;
		private IList<IInventoryBeverageLineItem> _lineItems;
		public InventoryVisuallyMeasureBottleViewModel(IAppNavigation navi,
			IInventoryService inventoryService, 
			ILoginService loginService, 
			ILogger logger) : base(navi, logger){

			_inventoryService = inventoryService;
			_loginService = loginService;

			_lineItems = new List<IInventoryBeverageLineItem>();

			PartialAmounts = new List<decimal> ();
			if (ResetMarkers != null) {
				ResetMarkers.Invoke ();
			}

			LoadLineItem ();
			DisplayName = CurrentLineItem.DisplayName;
			PropertyChanged += (sender, e) => {
				if (e.PropertyName == "CurrentPartial" || e.PropertyName == "NumFullBottles") {
					UpdateTotal ();
					AddPartialEnabled = CurrentPartial > 0;
				}

			};
		}
			
		public IInventoryBeverageLineItem CurrentLineItem{
			get{ return GetValue<IInventoryBeverageLineItem> ();}
			private set{ SetValue (value);}
		}
			
		public string DisplayName{ get{return GetValue<string> ();}set{ SetValue(value); }}

		public decimal Total{get{ return GetValue<decimal> (); }private set{ SetValue (value); }}

		public int NumFullBottles{	get { return GetValue<int>(); }
			set { SetValue<int>(value); }}

		public decimal CurrentPartial{get{return GetValue<decimal>();} set{SetValue(value);}}

		public bool AddPartialEnabled{ get { return GetValue<bool> (); } private set { SetValue<bool> (value); } }

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

		/// <summary>
		/// Fired to let us get the view model setup
		/// </summary>
		public override async Task OnAppearing(){
			//zero out every time
			PartialTotal = 0;
			PartialAmounts = new List<decimal> ();
			CurrentPartial = 0;
			NumFullBottles = 0;
			if (ResetMarkers != null) {
				ResetMarkers ();
			}
			AddPartialEnabled = false;
			UpdateTotal ();

			_lineItems = (await _inventoryService.GetLineItemsForCurrentSection ())
				.OrderBy(li => li.InventoryPosition).ToList();
			LoadLineItem ();
			DisplayName = CurrentLineItem.DisplayName;
		}

		#region Commands
		public ICommand MeasureCommand{get{ return new SimpleCommand (MeasureEv);
			}}

		public ICommand AddPartialCommand{get{ return new SimpleCommand (AddPartial, () => AddPartialEnabled);}}

		public ICommand CancelCommand{get{return new SimpleCommand (Cancel);}}

		public ICommand MoveNextCommand{ get { return new SimpleCommand (MoveNext, CanMoveNext); } }
		#endregion

		async void MoveNext(){
			if(_lineItems.Contains(CurrentLineItem)){
				//get the index
				var index = _lineItems.IndexOf (CurrentLineItem);
				var nextIndex = index + 1;
				//get the next
				if(_lineItems.Count > nextIndex + 1){
					//set in on service
					var nextItem = _lineItems[nextIndex];

					//save the current!
					Processing = true;
					await Measure();

					await _inventoryService.MarkLineItemForMeasurement(nextItem);
					CurrentLineItem = nextItem;

					//reload view
					await OnAppearing ();
					Processing = false;
				}
			}
		}

		bool CanMoveNext(){
			return _lineItems.Any() && CurrentLineItem != _lineItems.Last ();
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

		void AddPartial(){
			try{
				//see how many of our markers are set
				if (CurrentPartial > 0) {
					PartialAmounts.Add (CurrentPartial);
					PartialTotal += CurrentPartial;
					CurrentPartial = 0;

					if (ResetMarkers != null) {
						ResetMarkers.Invoke ();
					}
				}
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

		async void LoadLineItem ()
		{
			try{
				//load the line item here, or assume it's loaded elsewhere?
				CurrentLineItem = await _inventoryService.GetLineItemToMeasure ();
				if (CurrentLineItem == null) {
					await Navigation.CloseInventoryVisuallyMeasureItem ();
				}
				DisplayName = CurrentLineItem.DisplayName;
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

