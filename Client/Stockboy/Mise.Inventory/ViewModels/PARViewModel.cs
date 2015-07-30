using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using System;
using System.Collections.ObjectModel;
using Mise.Core.Services;
using Xamarin.Forms;
namespace Mise.Inventory.ViewModels
{
	public class PARLineItemDisplay : BaseLineItemDisplayLine<IPARBeverageLineItem>{
		public PARLineItemDisplay(IPARBeverageLineItem source) : base(source){}


		#region implemented abstract members of BaseLineItemDisplayLine
		public override Color TextColor {
			get {
				return Color.Default;
			}
		}
		public override string Quantity {
			get {
				return Source.Quantity.ToString();
			}
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
		#endregion
	}

	public class ParViewModel : BaseSearchableViewModel<PARLineItemDisplay>
	{
		readonly IPARService _parService;
		IPARBeverageLineItem _itemSettingQuantity;
		public ParViewModel(ILogger logger, IAppNavigation appNavigation, IPARService parService)
		:base(appNavigation, logger)
		{
			_parService = parService;
			_itemSettingQuantity = null;
		}
			
		#region Commands

		public ICommand AddNewItemCommand {
			get { return new Command(AddNewItem, () => NotProcessing); }
		}

		public ICommand SaveCommand {
			get { return new Command(Save, () => Processing == false); }
		}
			
		#endregion

		async void AddNewItem()
		{
			try{
				await Navigation.ShowPARItemFind ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void Save()
		{
			try{
				Processing = true;
				await _parService.SaveCurrentPAR ();
				Processing = false;
				await Navigation.ShowRoot();
			} catch(Exception e){
				HandleException (e);
			}
		}

		public override async Task SelectLineItem(PARLineItemDisplay displayLineItem){
			var lineItem = displayLineItem.Source;
			_itemSettingQuantity = lineItem;
			await _parService.SetCurrentLineItem (lineItem);
		    await Navigation.ShowUpdateParLineItem();
		}

		async void QuantitySetCallback(int newQuant, decimal ignore){
			if(_itemSettingQuantity != null)
			{
				if(_itemSettingQuantity.Quantity != newQuant){
					await _parService.UpdateQuantityOfPARLineItem (_itemSettingQuantity, newQuant);
				}
			}

			_itemSettingQuantity = null;
			await LoadItems ();
		}

		protected override async Task<ICollection<PARLineItemDisplay>> LoadItems(){
			var currPar = await _parService.GetCurrentPAR ();
			if(currPar == null){
				Processing = true;
				//create a new one
				await _parService.CreateCurrentPAR ();
				currPar = await _parService.GetCurrentPAR ();
				Processing = false;
			}

			if(currPar == null){
				throw new NullReferenceException ("Cannot find current par or create one!");
			}
			var items = currPar.GetBeverageLineItems ().ToList();

			return items.OrderBy (li => li.DisplayName).Select (li => new PARLineItemDisplay (li)).ToList();
		}

		protected override void AfterSearchDone ()
		{
		}
	}
}

