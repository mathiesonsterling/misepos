using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Mise.Core;
using Mise.Core.Services;
using Mise.Inventory.MVVM;
using Mise.Inventory.Services;

namespace Mise.Inventory.ViewModels
{
    /// <summary>
    /// Supports any screen that needs to have a 'Next' functionality
    /// </summary>
    public abstract class BaseNextViewModel<TItemType> : BaseViewModel where TItemType:class
    {
        protected BaseNextViewModel(IAppNavigation navigationService, ILogger logger) : base(navigationService, logger)
        {
        }

        protected abstract Task<IList<TItemType>> LoadItems();

        protected abstract Task BeforeMoveNext(TItemType currentItem);

        protected abstract Task AfterMoveNext(TItemType newItem);

        public TItemType CurrentItem { get { return GetValue<TItemType>(); } private set { SetValue(value); } }
        public TItemType NextItem { get { return GetValue<TItemType>(); } private set { SetValue(value);} }

        protected IList<TItemType> Items { get; private set; }

        public override async Task OnAppearing()
        {
            Items = await LoadItems();
            if (Items.Any() == false)
            {
                throw new Exception("No items found for this screen");
            }
			SetCurrent (Items.First ());
        }

        public ICommand MoveNextCommand { get{return new SimpleCommand(MoveNext, CanMoveNext);}}
	
        protected void SetCurrent(TItemType item)
        {
            CurrentItem = item;
            NextItem = GetNextItem();
        }

        private bool CanMoveNext()
        {
			if (Items == null) {
				return false;
			}
            if (Items.Contains(CurrentItem) == false)
            {
                Logger.Error("Error, item is not found in collection for BaseNextViewModel");
                return false;
            }

            return GetNextItem() != null;
        }

        private async void MoveNext()
        {
			if (CanMoveNext() == false) {
				return;
			}
            Processing = true;
            await BeforeMoveNext(CurrentItem);

            //get the next item
            var item = GetNextItem();
            SetCurrent(item);

            await AfterMoveNext(CurrentItem);
            Processing = false;
        }

        private TItemType GetNextItem()
        {
			//could be loading
			if (Items == null) {
				return null;
			}
            if (Items.Contains(CurrentItem) == false)
            {
                Logger.Error("Current item is not found in Items");
            }

            if (CurrentItem == Items.Last())
            {
                return null;
            }
            var currentIndex = Items.IndexOf(CurrentItem);
            var nextIndex = currentIndex + 1;
            return Items[nextIndex];
        }
    }
}
