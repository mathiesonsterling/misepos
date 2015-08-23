using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core.Entities.Inventory;
using Mise.Core.Services.UtilityServices;

using Mise.Inventory.Services;
using Xamarin.Forms;
using Mise.Core.Common.Services.WebServices.Exceptions;

namespace Mise.Inventory.ViewModels
{
	public class SectionSelectViewModel : BaseSearchableViewModel<IInventorySection>
	{
		readonly ILoginService _loginService;
		readonly IInventoryService _inventoryService;

		public SectionSelectViewModel(IAppNavigation appNavigation, ILogger logger, ILoginService loginService, 
			IInventoryService inventoryService) : base(appNavigation, logger)
		{
			_loginService = loginService;
			_inventoryService = inventoryService;
		}

	    #region Commands

		public ICommand AddSectionCommand {
			get { return new Command(AddSection, () => NotProcessing); }
		}

		public ICommand CompleteInventoryCommand{
			get{return new Command (CompleteInventory, CanCompleteInventory);}
		}

		async void AddSection()
		{
			try{
				await Navigation.ShowSectionAdd();
			} catch(Exception e){
				HandleException (e);
			}
		}

        public async void CompleteInventory()
        {
            try
            {
                Processing = true;
                //do we have all our sections done?
                var currInv = await _inventoryService.GetSelectedInventory();
                bool closeInv = true;
                var sectionsNotDone = currInv.GetSections().Where(sec => sec.Completed == false).ToList();
                if (sectionsNotDone.Any())
                {
                    var sectionsList = sectionsNotDone.Select(sec => sec.Name);
                    var sectionsString = string.Join(",", sectionsList);

                    var message = "You haven't done sections " + sectionsString + ".  Complete inventory anyways?";
					closeInv = true;//await Navigation.AskUser("Incomplete Sections", message);
                }

                if (closeInv)
                {
                    await _inventoryService.MarkInventoryAsComplete();
                    Processing = false;
                    await Navigation.ShowRoot();
                }
                Processing = false;
            }
			catch(DataNotSavedOnServerException des){
				HandleException (des, "Can't save data on the server - are you connected to the internet?");
			}
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        #endregion

        protected override async Task<ICollection<IInventorySection>> LoadItems()
        {
            Processing = true;
            var selectedInventory = await _inventoryService.GetSelectedInventory();
            if (selectedInventory == null)
            {
                throw new InvalidOperationException("No Inventory exists!");
            }
            Processing = false;
            var sections = selectedInventory.GetSections();
            return sections.ToList();
        }

        protected override void AfterSearchDone()
        {
        }


        public override async Task SelectLineItem(IInventorySection lineItem)
        {
            try
            {
                Processing = true;
                var emp = await _loginService.GetCurrentEmployee();
       
                if (lineItem.Completed && lineItem.LastCompletedBy != emp.ID)
                {
                    var userResponse =
                        await
                            Navigation.AskUser("Already counted!",
                                string.Format("This section has been completed by someone else.  Do you want to continue, and discard their count for {0}?", lineItem.Name));
                    if (userResponse == false)
                    {
                        return;
                    }
                }
                await _inventoryService.SetCurrentInventorySection(lineItem);
                Processing = false;
                await Navigation.ShowInventory();
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }


		bool CanCompleteInventory ()
		{
			var inv = _inventoryService.GetSelectedInventory ().Result;
			return inv != null;
		}
	}
}

