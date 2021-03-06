﻿using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.Services;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class SectionAddViewModel : BaseViewModel
	{
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

		public bool CanAdd{get{return GetValue<bool> ();}set{ SetValue (value); }}
		public SectionAddViewModel(IAppNavigation appNavigation, ILogger logger, 
			IInventoryService inventoryService) : base(appNavigation, logger)
		{
			_inventoryService = inventoryService;
			SectionHasPartialBottles = true;

			PropertyChanged += (sender, e) => {
				if(e.PropertyName == "SectionName"){
					CanAdd = AddCommand.CanExecute (null);
				}
			};
		}

		#region Commands

		public ICommand AddCommand {
			get { return new Command(AddSection, () => NotProcessing 
				&& string.IsNullOrWhiteSpace (SectionName) == false
				&& SectionName.Length > 1
			); }
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
				await Navigation.CloseSectionAdd();
			} catch(Exception e){
				HandleException (e);
			}
		}

        public override Task OnAppearing()
        {
			SectionName = string.Empty;
            return Task.FromResult(true);
        }
	}
}

