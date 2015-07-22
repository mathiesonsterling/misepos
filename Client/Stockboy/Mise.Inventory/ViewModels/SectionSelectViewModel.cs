using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services;
using Mise.Inventory.ViewModels;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;
using System.Collections.ObjectModel;

namespace Mise.Inventory.ViewModels
{
	public class SectionSelectViewModel : BaseViewModel
	{
		readonly ILoginService _loginService;
		readonly IInventoryService _inventoryService;
		public IEnumerable<IRestaurantInventorySection> Sections { get; set; }

		public SectionSelectViewModel(IAppNavigation appNavigation, ILogger logger, ILoginService loginService, 
			IInventoryService inventoryService) : base(appNavigation, logger)
		{
			_loginService = loginService;
			_inventoryService = inventoryService;
		}

		public override async Task OnAppearing(){
			try{
				await LoadPossible(_loginService);
			} catch(Exception e){
				HandleException (e);
			}
		}
		#region Commands

		public ICommand AddSectionCommand {
			get { return new SimpleCommand(AddSection); }
		}

		public ICommand SelectSectionCommand {
			get { return new SimpleCommand<IRestaurantInventorySection>(SelectSectionCom); }
		}

		public ICommand CompleteInventoryCommand{
			get{return new SimpleCommand (CompleteInventory, CanCompleteInventory);}
		}
		#endregion

		async void AddSection()
		{
			try{
				await Navigation.ShowSectionAdd();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async Task LoadPossible(ILoginService loginService)
		{
			var rest = await loginService.GetCurrentRestaurant();
			if (rest != null) {
				var secs = rest.GetInventorySections();
				Sections = secs;
			}

			//do we need to create an inventory?
		    Processing = true;
			var selectedInventory = await _inventoryService.GetSelectedInventory ();
		    Processing = false;
			if(selectedInventory == null){
				await _inventoryService.StartNewInventory ();
			}
		}

		public async void SelectSectionCom(IRestaurantInventorySection param){
			await SelectSection (param);
		}

		public async Task SelectSection(IRestaurantInventorySection param)
		{
			try{
				await _loginService.SelectSection (param);
				await Navigation.ShowInventory();
			}catch(Exception e){
				HandleException (e);
			}
		}

		public async void CompleteInventory ()
		{
			try
			{
			    Processing = true;
				//do we have all our sections done?
				var currInv = await _inventoryService.GetSelectedInventory ();
				bool closeInv = true;
				var sectionsNotDone = currInv.GetSections ().Where(sec => sec.Completed == false).ToList ();
				if(sectionsNotDone.Any()){
					var sectionsList = sectionsNotDone.Select (sec => sec.Name);
					var sectionsString = string.Join (",", sectionsList);

					var message = "You haven't done sections " + sectionsString + ".  Complete inventory anyways?";
					closeInv = await Navigation.AskUser ("Incomplete Sections", message);
				}

				if(closeInv){
					await _inventoryService.MarkInventoryAsComplete ();
			    	Processing = false;
					await Navigation.ShowRoot ();
				}
				Processing = false;
			} catch(Exception e){
				HandleException (e);
			}
		}


		bool CanCompleteInventory ()
		{
			var inv = _inventoryService.GetSelectedInventory ().Result;
			return inv != null;
		}
	}
}

