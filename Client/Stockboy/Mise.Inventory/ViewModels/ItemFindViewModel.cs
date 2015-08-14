using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;

using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Services;
using Xamarin.Forms;


namespace Mise.Inventory.ViewModels
{
	public class ItemFindViewModel : BaseSearchableViewModel<IBaseBeverageLineItem>
	{
		readonly IBeverageItemService _biService;
		readonly IInventoryService _inventoryService;
		readonly IPARService _parService;
		readonly IReceivingOrderService _roService;

		public bool CreateItemEnabled{get{return GetValue<bool> ();}set{ SetValue (value); }}

		public AddLineItemType CurrentType{get;set;}

		public ItemFindViewModel(IAppNavigation appNavigation, IBeverageItemService biService,
		IInventoryService inventoryService, IPARService parService, IReceivingOrderService roService, 
			ILogger logger) : base(appNavigation, logger)
		{
			_biService = biService;
			_inventoryService = inventoryService;
			_parService = parService;
			_roService = roService;
		}
			
		#region Commands

		public ICommand AddNewItemCommand {
			get { return new Command(AddNewItem, () => CreateItemEnabled); }
		}
			
		#endregion

		async void AddNewItem()
		{
			//add our search as the name to be helpful
			App.ItemAddViewModel.Name = SearchString;
			switch(CurrentType){
			case AddLineItemType.Inventory:
				await Navigation.ShowInventoryItemAdd ();
				break;
			case AddLineItemType.ReceivingOrder:
				await Navigation.ShowReceivingOrderItemAdd ();
				break;
			case AddLineItemType.PAR:
				await Navigation.ShowPARItemAdd ();
				break;
			default:
				throw new ArgumentException ("Invalid AddLineItemType selected!");
			}
		}

		public override async Task SelectLineItem(IBaseBeverageLineItem lineItem){
			try{
				//based on our type, determine what LI to add
				switch(CurrentType){
				case AddLineItemType.Inventory:
					await _inventoryService.AddLineItemToCurrentInventory (lineItem, 0);
					break;
				case AddLineItemType.ReceivingOrder:
					await _roService.AddLineItemToCurrentReceivingOrder (lineItem, 0);
					break;
				case AddLineItemType.PAR:
					await _parService.AddLineItemToCurrentPAR (lineItem, null);
					break;
				default:
					throw new ArgumentException ("Invalid AddLineItemType selected!");
				}
				//and return to our home base
				await Navigation.CloseItemFind ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		protected override async Task<ICollection<IBaseBeverageLineItem>> LoadItems(){
			//get the items
			try{
				Processing = true;
			   var bis = await _biService.GetPossibleItems ();
				Processing = false;
				//we need to remove the items that are currently already in our item!
				switch(CurrentType){
				case AddLineItemType.Inventory:
					//don't filter the items, we want to be able to add multiple copies of them
					/*
					var currInv = await _inventoryService.GetSelectedInventory ();
					if (currInv != null)
					{
					    var invSection = await _inventoryService.GetCurrentInventorySection();
						if (invSection != null) {
							var existing = invSection.GetInventoryBeverageLineItemsInSection ();
							bis = ExcludeItems (bis, existing.ToList<IBaseBeverageLineItem>());
						}
					}*/
					break;
				case AddLineItemType.PAR:
					var currPar = await _parService.GetCurrentPAR ();
					if (currPar != null) {
						var existing = currPar.GetBeverageLineItems ();
						bis = ExcludeItems (bis, existing.ToList<IBaseBeverageLineItem>());
					}
					break;
				}

				var items = bis.ToList ();
				return items;
			} catch(Exception e){
				HandleException (e);
				return new List<IBaseBeverageLineItem> ();
			}
		}

		static IEnumerable<IBaseBeverageLineItem> ExcludeItems(IEnumerable<IBaseBeverageLineItem> list, ICollection<IBaseBeverageLineItem> toExclude){
			var excluded = new List<IBaseBeverageLineItem> ();
			foreach(var bi in list){
				if(IsBIInList (toExclude, bi) == false){
					excluded.Add (bi);
				}
			}

			return excluded;
		}

		static bool IsBIInList(IEnumerable<IBaseBeverageLineItem> list, IBaseBeverageLineItem bi){
			foreach(var item in list){
				if(BeverageLineItemEquator.AreSameBeverageLineItem (item, bi)){
					return true;
				}
			}
			return false;
		}

		protected override void AfterSearchDone ()
		{
			CreateItemEnabled = LineItems.Any () == false;
		}
	}
}

