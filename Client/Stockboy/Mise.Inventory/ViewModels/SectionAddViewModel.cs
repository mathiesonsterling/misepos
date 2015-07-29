using System;
using System.Windows.Input;
using System.Threading.Tasks;

using Mise.Inventory.Services;
using Mise.Core.Services;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class SectionAddViewModel : BaseViewModel
	{
		readonly ILoginService _loginService;
		readonly IInventoryService _inventoryService;

		public string SectionName{get{return GetValue<string> ();}set{ SetValue (value); }}
		/// <summary>
		/// If true, the section we're creating allows partial bottles
		/// </summary>
		public bool SectionHasPartialBottles{ get; set;}

		/// <summary>
		/// If true, the section being created will be our new default to places items from ROs, removing any previous
		/// </summary>
		public bool IsDefaultInventorySection{ get; set;}

		public SectionAddViewModel(IAppNavigation appNavigation, ILoginService loginService, ILogger logger, 
			IInventoryService inventoryService) : base(appNavigation, logger)
		{
			_loginService = loginService;
			_inventoryService = inventoryService;
			SectionHasPartialBottles = true;
		}

		#region Commands

		public ICommand AddCommand {
			get { return new Command(AddSection, () => NotProcessing); }
		}

		#endregion

		public async void AddSection()
		{
			//TODO - if we've got a working UI element, show here!
			try{
				Processing = true;
				await _inventoryService.AddNewSection (SectionName, SectionHasPartialBottles, IsDefaultInventorySection);
			
				SectionName = string.Empty;
				SectionHasPartialBottles = true;
				IsDefaultInventorySection = false;

				Processing = false;
				await Navigation.ShowSectionSelect ();
			} catch(Exception e){
				HandleException (e);
			}
		}

        public override Task OnAppearing()
        {
            return Task.FromResult(false);
        }
	}
}

