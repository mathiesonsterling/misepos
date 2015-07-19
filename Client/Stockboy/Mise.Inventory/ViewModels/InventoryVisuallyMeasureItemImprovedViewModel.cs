using System;
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

namespace Mise.Inventory.ViewModels
{
	public class InventoryVisuallyMeasureItemImprovedViewModel : BaseViewModel
	{
		public IInventoryBeverageLineItem LineItem{
			get{ return GetValue<IInventoryBeverageLineItem> ();}
			set{ SetValue (value);}}

		public string DisplayName{ get{return GetValue<string> ();}set{ SetValue(value); }}

		public decimal Total{get{ return GetValue<decimal> (); }set{ SetValue (value); }}

		public int NumFullBottles{	get { return GetValue<int>(); }
			set { SetValue<int>(value); }}

		public decimal CurrentPartial{get{return GetValue<decimal>();} set{SetValue(value);}}

		public bool AddPartialEnabled{ get { return GetValue<bool> (); } set { SetValue<bool> (value); } }

		public decimal PartialTotal{
			get{return GetValue<decimal>();}
			set{ SetValue(value); }
		}

		public List<decimal> PartialAmounts{
			get{ return GetValue<List<decimal>> (); }
			set{ SetValue (value); }
		}
			
		public Action ResetMarkers{ get; set; }

		readonly IInventoryService _inventoryService;
		readonly ILoginService _loginService;
		public InventoryVisuallyMeasureItemImprovedViewModel(IAppNavigation navi,
			IInventoryService inventoryService, 
			ILoginService loginService, 
			ILogger logger) : base(navi, logger){

			_inventoryService = inventoryService;
			_loginService = loginService;

			PartialAmounts = new List<decimal> ();
			if (ResetMarkers != null) {
				ResetMarkers.Invoke ();
			}

			LoadLineItem ();
			DisplayName = LineItem.DisplayName;
			PropertyChanged += (sender, e) => {
				if (e.PropertyName == "CurrentPartial" || e.PropertyName == "NumFullBottles") {
					UpdateTotal ();
						AddPartialEnabled = CurrentPartial > 0;
				}

			};
		}


		void UpdateTotal(){
			Total = NumFullBottles + PartialTotal;
		}
				
		/// <summary>
		/// Fired to let us get the view model setup
		/// </summary>
		public override Task OnAppearing(){
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

			LoadLineItem ();
			DisplayName = LineItem.DisplayName;

		    return Task.FromResult(true);
		}

		#region Commands
		public ICommand MeasureCommand{get{ return new SimpleCommand (Measure);
			}}

		public ICommand AddPartialCommand{get{ return new SimpleCommand (AddPartial);}}

		public ICommand CancelCommand{get{return new SimpleCommand (Cancel);}}
		#endregion

		async void Measure(){
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
				LineItem = await _inventoryService.GetLineItemToMeasure ();
				if (LineItem == null) {
					await Navigation.CloseInventoryVisuallyMeasureItem ();
				}
				DisplayName = LineItem.DisplayName;
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

