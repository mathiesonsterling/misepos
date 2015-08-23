using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using System;
using Xamarin.Forms;
namespace Mise.Inventory.ViewModels
{
	public class ParViewModel : BaseSearchableViewModel<ParViewModel.ParLineItemDisplay>
	{
		public class ParLineItemDisplay : BaseLineItemDisplayLine<IParBeverageLineItem>{
			public ParLineItemDisplay(IParBeverageLineItem source) : base(source){}


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

		readonly IPARService _parService;
		public ParViewModel(ILogger logger, IAppNavigation appNavigation, IPARService parService)
		:base(appNavigation, logger)
		{
			_parService = parService;
		}
			
		#region Commands

		public ICommand AddNewItemCommand {
			get { return new Command(AddNewItem, () => NotProcessing); }
		}

		public ICommand SaveCommand {
			get { return new Command(Save, () => NotProcessing); }
		}

		public ICommand DeleteLineItemCommand{get{return new Command<ParLineItemDisplay> (DeleteLineItemE, CanDeleteLineItem);}}
		#endregion

		async void AddNewItem()
		{
			try{
				await Navigation.ShowPARItemFind ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async void DeleteLineItemE(ParLineItemDisplay lineItem){
			await DeleteLineItem (lineItem);
		}

		public async Task DeleteLineItem(ParLineItemDisplay lineItem){

		}

		public bool CanDeleteLineItem(ParLineItemDisplay lineItem){
			return NotProcessing;
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

		public override async Task SelectLineItem(ParLineItemDisplay lineItem){
			var realLI = lineItem.Source;
			await _parService.SetCurrentLineItem (realLI);
		    await Navigation.ShowUpdateParLineItem();
		}
			
		protected override async Task<ICollection<ParLineItemDisplay>> LoadItems(){
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

			var lastItem = items.OrderByDescending (li => li.LastUpdatedDate).FirstOrDefault ();
			FocusedItem = new ParLineItemDisplay (lastItem);

			return items.OrderBy (li => li.DisplayName).Select (li => new ParLineItemDisplay (li)).ToList();
		}

		protected override void AfterSearchDone ()
		{
		}
	}
}

