using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using Mise.Core;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;
using Mise.Core.Services;

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
			
		public InventoryViewModel(IAppNavigation appNavigation, ILoginService loginService, 
			IInventoryService inventoryService, ILogger logger) : base(appNavigation, logger)
		{
			_inventoryService = inventoryService;
			_loginService = loginService;
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
                var section = await _loginService.GetCurrentSection();
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

        public InventoryLineItemDisplayLine FirstUnmeasuredItem { get { return GetValue<InventoryLineItemDisplayLine>(); } private set { SetValue(value);} }

		public bool CanComplete{ get { return GetValue<bool> (); } private set { SetValue (value); } }

        public bool CameFromAdd { get; private set; }

		#region Commands

		public ICommand AddNewLineItemCommand {
			get { return new SimpleCommand(AddNewItem); }
		}

		public ICommand ScanCommand {
			get { return new SimpleCommand(Scan); }
		}

		public ICommand FinishSectionCommand{
			get{return new SimpleCommand (FinishSection);}
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
					CanComplete = NotProcessing && LineItems.Any () && FirstUnmeasuredItem == null;
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

		protected override async Task<ICollection<InventoryLineItemDisplayLine>> LoadItems (){
			var items = (await _inventoryService.GetLineItemsForCurrentSection ()).ToList();
            var itemsList = items.OrderBy(li => li.InventoryPosition).Select(li => new InventoryLineItemDisplayLine(li)).ToList();
            //find the first unmeasured
		    FirstUnmeasuredItem = itemsList.FirstOrDefault(li => li.Source.HasBeenMeasured == false);

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
			} catch(Exception e){
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

