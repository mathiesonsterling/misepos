using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.Services;
using Mise.Core.ValueItems.Inventory;
using System.Threading.Tasks;
using Mise.Core;
using Mise.Core.Entities.Inventory;
using Xamarin.Forms;

namespace Mise.Inventory.ViewModels
{
	public class ItemAddViewModel : BaseViewModel
	{
			
		IEnumerable<LiquidContainer> PossibleContainers{ get; set;}
		IEnumerable<ICategory> PossibleCategories{get;set;}

		public AddLineItemType CurrentAddType{ get; set;}

		readonly IBeverageItemService _biService;
		readonly IInventoryService _inventoryService;
		readonly IReceivingOrderService _roService;
		readonly IPARService _parService;
		readonly ICategoriesService _categoriesService;
		public ItemAddViewModel(IAppNavigation appNavigation, IBeverageItemService biService, 
			IInventoryService inventoryService, IReceivingOrderService roService, IPARService parService,
			ICategoriesService categoriesService, ILogger logger) : base(appNavigation, logger)
		{
			_biService = biService;
			_inventoryService = inventoryService;
			_roService = roService;
			_parService = parService;
			_categoriesService = categoriesService;

			PropertyChanged += (sender, e) => {
				if(e.PropertyName != "CreateEnabled")
				{
					CreateEnabled = (string.IsNullOrEmpty (Name) == false)
						&& (string.IsNullOrEmpty (SelectedContainerName) == false)
						&& CaseSize > 0
						&& (string.IsNullOrEmpty (SelectedCategoryName) == false);
				}
			};
				
		}

		public override async Task OnAppearing(){
			ParNumber = 0;
			CaseSize = 12;
			CaseSize = 12;
			CreateEnabled = false;
			SelectedContainerName = string.Empty;
			SelectedCategoryName = string.Empty;

			await LoadPossibleItems ();
		}

		#region Fields
		public int CaseSize{
			get{return GetValue<int> ();}
			set{ SetValue (value); }
		}
		public string Name{
			get{return GetValue<string> ();}
			set{ SetValue (value); }
		}
		public string SelectedContainerName{
			get{return GetValue<string> ();}
			set{ SetValue (value); }
		}

		public string SelectedCategoryName{
			get{return GetValue<string> ();}
			set{ SetValue (value); }
		}

		public int? AddAtPosition{ get { return GetValue<int?> (); } set { SetValue (value); } }

		/// <summary>
		/// If set, we wish to create a par item for this as well
		/// </summary>
		/// <value>The par number.</value>
		public int ParNumber{
			get{return GetValue<int> ();}
			set{ SetValue (value); }
		}

		public bool CreateEnabled{
			get{ return GetValue<bool> (); }
				private set{ SetValue (value); }
		}

		public bool AddToParEnabled => true;
	    public IEnumerable<string> PossibleContainerNames{ get; set;}

		public IEnumerable<string> PossibleCategoryNames{get;set;}
		#endregion

		#region Commandsk

		public ICommand ScanCommand {
			get { return new Command(Scan, ()=>NotProcessing); }
		}

		public ICommand AddCommand {
			get { return new Command(Add, () => CreateEnabled); }
		}
			
		#endregion

		async void Add()
		{
			try{
				Processing = true;
				var container = PossibleContainers.FirstOrDefault (c => c.DisplayName == SelectedContainerName);

				var category = PossibleCategories.FirstOrDefault (c => c.Name == SelectedCategoryName);
				//we need to know what kind of item we're trying to add here
				//add it, then return based on that!
				switch(CurrentAddType){
					case AddLineItemType.Inventory:
					var invItem = await _inventoryService.AddLineItemToCurrentInventory (Name, category, null, 0, CaseSize,
						container, AddAtPosition);
					if(ParNumber > 0 && invItem != null){
						await _parService.AddLineItemToCurrentPAR (invItem, ParNumber);
					}
						break;
					case AddLineItemType.ReceivingOrder:
						var roItem = await _roService.AddLineItemToCurrentReceivingOrder (Name, category, null, 0, CaseSize, 
						container);
					if(ParNumber > 0 && roItem != null){
						await _parService.AddLineItemToCurrentPAR (roItem, ParNumber);
					}
					// TODO go to quantity
						break;
				case AddLineItemType.PAR:
						await _parService.AddLineItemToCurrentPAR (Name, category, null, ParNumber, CaseSize, container);
					//TODO go to quantity
						break;
					default:
						throw new ArgumentException ("Error, type of item to add has not been set");
				}
				Processing = false;
				AddAtPosition = null;
                //we added, we need to update our beverage item cache
			    await _biService.ReloadItemCache();
				await Navigation.CloseItemAdd ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		async Task LoadPossibleItems ()
		{
			//curretly only handles one level of subcats
			PossibleCategories = _categoriesService.GetIABIngredientCategories ();

			var containers = (await _biService.GetAllPossibleContainerSizes ()).ToList();
			PossibleContainers = containers;
			PossibleContainerNames = containers.Select (c => c.DisplayName);

			var names = PossibleCategories.Select (c => c.Name)
				.OrderByDescending (s => s.ToUpper () == "NONE")
				.ThenBy (s => s);
			//arrange the names how?

			PossibleCategoryNames = names;
		}

		async void Scan()
		{
			await Navigation.ShowItemScan();
		}
	}
}

