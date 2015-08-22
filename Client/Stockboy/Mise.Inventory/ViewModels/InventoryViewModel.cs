using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using Mise.Core;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;
using Mise.Core.Services;
using Mise.Core.Common.Services.WebServices.Exceptions;
using Mise.Core.Common.Events.Inventory;

namespace Mise.Inventory.ViewModels
{
	public class InventoryViewModel : BaseSearchableViewModel<InventoryViewModel.InventoryLineItemDisplayLine>
	{
        /// <summary>
        /// Adapter class to let us specify how the item is to be displayed
        /// </summary>
	    public class InventoryLineItemDisplayLine : BaseLineItemDisplayLine<IInventoryBeverageLineItem>
        {

	        public InventoryLineItemDisplayLine(IInventoryBeverageLineItem source) : base(source)
	        {
	        }

	        public override string Quantity
	        {
				get { return Source.Quantity.ToString(); }
	        }

	        public override Color TextColor
	        {
	            get { return Source.HasBeenMeasured ? Color.Default : Color.Accent; }
	        }

			public override string DetailDisplay {
				get {
					var res = string.Empty;
					if(Source.GetCategories ().Any()){
						res += Source.GetCategories ().First ().Name;
					}
					if(Source.Container != null){
						res += "  " + Source.Container.DisplayName;
					}
					return res;
				}
			}
	    }

		readonly IInventoryService _inventoryService;
		readonly ILoginService _loginService;
		readonly IList<Guid> _lineItemsCurrentlyChanging;
		public InventoryViewModel(IAppNavigation appNavigation, ILoginService loginService, 
			IInventoryService inventoryService, ILogger logger) : base(appNavigation, logger)
		{
			_inventoryService = inventoryService;
			_loginService = loginService;
			_lineItemsCurrentlyChanging = new List<Guid> ();
		}

        public override async Task OnAppearing()
        {
            try
            {
                if (CameFromAdd)
                {
                    CameFromAdd = false;
                }
                await base.OnAppearing();
                var section = await _inventoryService.GetCurrentInventorySection();
                if (section != null)
                {
                    Title = "Count for " + section.Name;
                }
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

		public string Title{get{ return GetValue<string> (); } private set{ SetValue (value); }}
		public bool IsInventoryEmpty{get{ return GetValue<bool> (); }private set{SetValue (value);}}

		public bool CanComplete{ get { return GetValue<bool> (); } private set { SetValue (value); } }

        public bool CameFromAdd { get; private set; }

		#region Commands

		public ICommand AddNewLineItemCommand {
			get { return new Command(AddNewItem, () => NotProcessing); }
		}

		public ICommand ScanCommand {
			get { return new Command(Scan, () => NotProcessing); }
		}

		public ICommand FinishSectionCommand{
			get{return new Command (FinishSection, () => CanComplete);}
		}

		public ICommand DeleteLineItemCommand{
			get{return new Command<InventoryLineItemDisplayLine> (DeleteLineItemT, (item) => CanDeleteLI(item));}
		}
		#endregion

		public override async Task SelectLineItem(InventoryLineItemDisplayLine lineItem){
			try
			{
			    var inventoryLineItem = lineItem.Source;
				//mark as selected, and move	
				await _inventoryService.MarkLineItemForMeasurement (inventoryLineItem);
				await Navigation.ShowInventoryVisuallyMeasureItem ();
			} catch(Exception e){
				HandleException (e);
			}

			PropertyChanged += (sender, e) => {
				if(e.PropertyName != "CanComplete"){
					CanComplete = NotProcessing && LineItems.Any () && FocusedItem == null;
				}
			};
		}

		async void AddNewItem()
		{
		    CameFromAdd = true;
			await Navigation.ShowInventoryItemFind ();
		}

		async void Scan()
		{
			await Navigation.ShowItemScan();
		}

		async void DeleteLineItemT(InventoryLineItemDisplayLine displayLI){
			await DeleteLineItem (displayLI);
		}

	
		public bool CanDeleteLI(InventoryLineItemDisplayLine li){
			return NotProcessing && (_lineItemsCurrentlyChanging.Contains(li.ID) == false);
		}

		public async Task DeleteLineItem (InventoryLineItemDisplayLine displayLI)
		{
			try{

				var ask = await Navigation.AskUser ("Delete Item", "Remove " + displayLI.DisplayName + " from section?");
				if(ask){
					_lineItemsCurrentlyChanging.Add(displayLI.ID);
					var li = displayLI.Source;

					await _inventoryService.DeleteLineItem (li);

					_lineItemsCurrentlyChanging.Remove(displayLI.ID);
					await DoSearch();
				}
			} catch(Exception e){
				HandleException (e);
			}
		}

		protected override async Task<ICollection<InventoryLineItemDisplayLine>> LoadItems (){
			var items = (await _inventoryService.GetLineItemsForCurrentSection ()).ToList();
            var itemsList = items.OrderBy(li => li.InventoryPosition)
				.ThenBy (li => li.DisplayName)
				.Select(li => new InventoryLineItemDisplayLine(li)).ToList();
            //find the first unmeasured
		    

			if(CameFromAdd){
				//get the last updated item
				var lastUpdated = items.OrderByDescending (li => li.LastUpdatedDate)
					.FirstOrDefault ();
				FocusedItem = lastUpdated != null ? new InventoryLineItemDisplayLine (lastUpdated) : null;
			} else{
				FocusedItem = itemsList
					.FirstOrDefault(li => li.Source.HasBeenMeasured == false);
			}

			IsInventoryEmpty = itemsList.Any () == false;
			//we can complete if we have items, and all are measured
			return itemsList;
		}

		async void FinishSection(){
			try{
				Processing = true;
				await _inventoryService.MarkSectionAsComplete ();
				Processing = false;
				//pop us back to our caller (select section)
				await Navigation.ShowSectionSelect ();
			} 
			catch(DataNotSavedOnServerException des){
				HandleException (des, "Can't save data on the server - are you connected to the internet?");
			}
			catch(Exception e){
				HandleException (e);
			}
		}

		#region implemented abstract members of BaseLineItemViewModel

		protected override void AfterSearchDone ()
		{
		}

		#endregion
	}
}

