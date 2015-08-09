using System;
using System.Linq;
using System.Collections.Generic;
using Mise.Core.Services.UtilityServices;
using Mise.Inventory.ViewModels;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Mise.Core;
using Mise.Inventory.Services;
using Mise.Core.Services;

namespace Mise.Inventory.ViewModels
{
	public abstract class BaseSearchableViewModel<TLineItemType> : BaseViewModel where TLineItemType : ITextSearchable
	{
		protected BaseSearchableViewModel(IAppNavigation navigation, ILogger logger) : base(navigation, logger)
		{
			PropertyChanged += async (sender, e) => {
				try{
					if(e.PropertyName == "SearchString"){
						if(LastSearchString != SearchString){
							LastSearchString = SearchString;
							await DoSearch ();
						}
					}
				} catch(Exception ex){
					HandleException (ex);
				}
			};
		}

		private async Task DoSearch(){
			try{
				var items = await LoadItems();
				if(string.IsNullOrEmpty (SearchString) == false){
					//split the search into items
					var searchTerms = SearchString.Split (' ');
					foreach(var searchTerm in searchTerms){
						var held = searchTerm.Trim ();
						if (string.IsNullOrEmpty (held) == false) {
							items = items.Where (li => li.ContainsSearchString (held)).ToList ();
						}
					}
				}
				LineItems = items;
				if(LoadItemsOnView  != null){
					LoadItemsOnView();
				}
				AfterSearchDone ();
			} catch(Exception e){
				HandleException (e);
			}
		}

		public IEnumerable<TLineItemType> LineItems{ get; private set;}

		/// <summary>
		/// The item that the display should focus on
		/// </summary>
		/// <value>The focused item.</value>
		public TLineItemType FocusedItem{ get{return GetValue<TLineItemType> ();} protected set{ SetValue (value); }}

		protected string LastSearchString{ get; private set;}
		public string SearchString {
			get{return GetValue<string> ();}
			set{ SetValue (value); }
		}
			
		public abstract Task SelectLineItem (TLineItemType lineItem);

		public override async Task OnAppearing(){
			SearchString = string.Empty;
			await DoSearch ();
		}

		protected abstract Task<ICollection<TLineItemType>> LoadItems ();

		/// <summary>
		/// Signals our instantiation our search is done and we can update
		/// </summary>
		/// <returns>The search done.</returns>
		protected abstract void AfterSearchDone ();

		/// <summary>
		/// Callback for the page to provide us to allow reloading manually
		/// </summary>
		/// <value>The load items on view.</value>
		public Action LoadItemsOnView{ private get; set;}
	}
}

