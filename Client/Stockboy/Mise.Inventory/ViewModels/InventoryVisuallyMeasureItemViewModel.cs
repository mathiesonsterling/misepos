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
	public class InventoryVisuallyMeasureItemViewModel : BaseViewModel
	{
		public IInventoryBeverageLineItem LineItem{get;set;}

		public string DisplayName{ get{return GetValue<string> ();}set{ SetValue(value); }}

		public decimal Total{get{ return GetValue<decimal> (); }set{ SetValue (value); }}

		public int NumFullBottles{	get { return GetValue<int>(); }
			set { SetValue<int>(value); }}

		public decimal CurrentPartial{ 			get { return GetValue<decimal>(); }
			set { SetValue<decimal>(value); }}

		public bool AddPartialEnabled{ get { return GetValue<bool> (); } set { SetValue<bool> (value); } }
			
		public List<decimal> PartialBottles{
			get{return GetValue<List<decimal>>();}
			set{ SetValue(value); }
		}

		readonly IInventoryService _inventoryService;
		readonly ILoginService _loginService;
		public InventoryVisuallyMeasureItemViewModel(IAppNavigation navi,IInventoryService inventoryService, 
			ILoginService loginService, ILogger logger) : base(navi, logger)
		{
			_inventoryService = inventoryService;
			_loginService = loginService;

			PartialBottles = new List<decimal> ();
			LoadLineItem ();

			PropertyChanged += (sender, e) => {
				if (e.PropertyName == "CurrentPartial" || e.PropertyName == "NumFullBottles") {
					UpdateTotal ();
				}
			};
		}

		void UpdateTotal(){
			if (CurrentPartial > 0) {
				Total = NumFullBottles + CurrentPartial;
				AddPartialEnabled = true;
			} else {
				AddPartialEnabled = false;
				Total = NumFullBottles;
			}
		}

		/// <summary>
		/// Fired to let us get the view model setup
		/// </summary>
		public override Task OnAppearing(){
			LoadLineItem ();
			NumFullBottles = LineItem.NumFullBottles;
			if(LineItem.NumPartialBottles > 0){
				CurrentPartial = LineItem.PartialBottlePercentages.First();
				PartialBottles = LineItem.PartialBottlePercentages.ToList();
			}
			Total = NumFullBottles + CurrentPartial;
			DisplayName = LineItem.DisplayName;

		    return Task.FromResult(true);
		}

		#region Commands
		public ICommand MeasureCommand{get{ return new SimpleCommand (Measure);
			}}
		
		public ICommand AddPartialCommand{get{ return new SimpleCommand (AddPartial);}}
		#endregion

		async void Measure(){
			try{
				//TODO we're done, fire events to measure and move back to the inventory!
				if(CurrentPartial > 0){
					PartialBottles = new List<decimal>{ CurrentPartial };
				}
				await _inventoryService.MeasureCurrentLineItemVisually (NumFullBottles, PartialBottles);
				NumFullBottles = 0;
				PartialBottles = new List<decimal>();
				CurrentPartial = 0;

				//we have to load here, otherwise the values won't update
				await Navigation.CloseInventoryVisuallyMeasureItem ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		void AddPartial(){
			try{
				PartialBottles.Add (CurrentPartial);
				CurrentPartial = 0;
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
			} catch(Exception e){
				HandleException (e);
			}
		}
	}
}

