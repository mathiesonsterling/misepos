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
using Xamarin.Forms;

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

        protected abstract Task BeforeMove(TItemType currentItem);
        protected abstract Task AfterMove(TItemType newItem);

		public Func<Task> MoveNextAnimation{ get; set;}
		public Func<Task> ResetViewAnimation{ get; set;}
		public Func<Task> MovePreviousAnimation{get;set;}

        public TItemType CurrentItem { get { return GetValue<TItemType>(); } private set { SetValue(value); } }
        public TItemType NextItem { get { return GetValue<TItemType>(); } private set { SetValue(value);} }
		public TItemType PreviousItem{get{return GetValue<TItemType> ();}private set{ SetValue (value); }}

		public bool CanMoveToNext{ get { return GetValue<bool> (); } private set { SetValue (value); } }
		public bool CanMoveToPrevious{get{ return GetValue<bool> (); }private set{ SetValue (value); }}

        protected IList<TItemType> Items { get; private set; }

        public override async Task OnAppearing()
        {
            Processing = true;
            Items = await LoadItems();
            if (Items.Any() == false)
            {
                Processing = false;
                throw new Exception("No items found for this screen");
            }
			SetCurrent (Items.First ());
            Processing = false;
        }

        public ICommand MoveNextCommand { get{return new SimpleCommand(MoveNext, CanMoveNext);}}
		public ICommand MovePreviousCommand{get{return new SimpleCommand (MovePrevious, CanMovePrevious);}}

        protected void SetCurrent(TItemType item)
        {
            CurrentItem = item;
            NextItem = GetNextItem();
			CanMoveToNext = NotProcessing && NextItem != null;
			PreviousItem = GetPreviousItem ();
			CanMoveToPrevious = NotProcessing && PreviousItem != null;
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

		bool CanMovePrevious(){
			if(Items == null){
				return false;
			}

			if (Items.Contains(CurrentItem) == false)
			{
				Logger.Error("Error, item is not found in collection for BaseNextViewModel");
				return false;
			}

			return GetPreviousItem () != null;
		}

        private async void MoveNext()
        {
			if (CanMoveNext() == false) {
				return;
			}
			if(MoveNextAnimation != null){
				await MoveNextAnimation ();
			}
            Processing = true;
            await BeforeMove(CurrentItem);

            //get the next item
            var item = GetNextItem();
            SetCurrent(item);

            await AfterMove(CurrentItem);
			if(ResetViewAnimation != null){
				await ResetViewAnimation ();
			}
            Processing = false;
        }

		private async void MovePrevious(){
			if(CanMovePrevious () == false){
				return;
			}
			if(MovePreviousAnimation != null){
				await MovePreviousAnimation ();
			}
			Processing = true;
			await BeforeMove (CurrentItem);

			var item = GetPreviousItem ();
			SetCurrent (item);

			await AfterMove (CurrentItem);
			if(ResetViewAnimation != null){
				await ResetViewAnimation ();
			}
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

		private TItemType GetPreviousItem(){
			//could be loading
			if (Items == null) {
				return null;
			}
			if (Items.Contains(CurrentItem) == false)
			{
				Logger.Error("Current item is not found in Items");
			}

			if(CurrentItem == Items.First ()){
				return null;
			}

			var currentIndex = Items.IndexOf(CurrentItem);
			if(currentIndex == 0){
				return null;
			}
			var nextIndex = currentIndex - 1;
			return Items[nextIndex];
		}
    }
}
