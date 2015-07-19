using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace Mise.POSTerminal.ViewModels
{
	/// <summary>
	/// Gotta start to reign in some of this complexity!  Translate model behavior to view
	/// </summary>
	public class OrderOnCheckViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{
			throw new NotImplementedException();
		}

		#endregion

		bool _modifierActive;

		public bool ModifierActive { 
			get{ return _modifierActive; } 
			set { 
				_modifierActive = value;
				OnPropertyChanged("ModifierActive");
			}
		}

		/// <summary>
		/// When an order is being put in, but has required mods and hasn't completed in one button click
		/// </summary>
		public bool CurrentlyOrdering { get; set; }

		/// <summary>
		/// If there is any amount due on the current check
		/// </summary>
		/// <value><c>true</c> if this instance has charges; otherwise, <c>false</c>.</value>
		public bool HasCharges {
			get {
				return Model.SelectedCheck != null && Model.SelectedCheck.Total != null &&
				Model.SelectedCheck.Total.HasValue;
			}
		}

		MenuItemCategory _selectedCategory;

		public MenuItemCategory SelectedCategory {
			get { return _selectedCategory; }
			set {
				_selectedCategory = value;
				OnPropertyChanged("SelectedCategory");
			}
		}

		public ICheck SelectedCheck { get { return Model.SelectedCheck; } }

		public IEnumerable<MenuItemCategory> CurrentSubCategories { get { return Model.CurrentSubCategories; } }

		public bool MiseIsVisible {
			get { return Model.SelectedCategory == null; }
		}

		public bool CategoryIsShowing {
			get { return Model.SelectedCategory != null; }
		}

		public IEnumerable<Mise.Core.Entities.Menu.MenuItem> MiseItems {
			get { return Model.MiseItems; }
		}

		public IEnumerable<Mise.Core.Entities.Menu.MenuItem> HotItems { get { return Model.HotItemsUnderCurrentCategory.Take(3); } }

		public IEnumerable<Tuple<MenuItemCategory, IEnumerable<Mise.Core.Entities.Menu.MenuItem>>> GetCategoryNamesAndMenuItems(int maxItemsOnScreen, int? maxItemsInRow)
		{
			return Model.GetCategoryNamesAndMenuItems(maxItemsOnScreen, maxItemsInRow);
		}

		public OrderItem SelectedOrderItem {
			get{ return Model.SelectedOrderItem; }
		}

		public bool IsModifyingSentOrderItem {
			get{ return SelectedOrderItem != null && SelectedOrderItem.Status == OrderItemStatus.Sent; }
		}

		/// <summary>
		/// Event for us to signal the screen needs to update the state of commands
		/// </summary>
		public event ViewModelEventHandler OnUpdateCommands;
		public event ViewModelEventHandler OnLoadOrderItems;
		public event ViewModelEventHandler OnLoadCategories;
		public event ViewModelEventHandler OnLoadMenuItems;
		public event ViewModelEventHandler OnLoadModifiers;
		public event ViewModelEventHandler OnLoadHotItems;
		public event ViewModelEventHandler OnModifierModeChanged;

		public ICommand FastCash{ get; private set; }

		public ICommand Close{ get; private set; }

		public ICommand Cancel{ get; private set; }

		public ICommand Send{ get; private set; }

		public ICommand Extra{ get; private set; }

		public ICommand MenuItemClicked{ get; private set; }

		public ICommand CategoryClicked { get; private set; }

		public ICommand CategoryUpClicked{ get; private set; }

		public ICommand CategoryHomeClicked{ get; private set; }

		public ICommand CompSelectedOrderItem{ get; private set; }

		public ICommand UndoCompedSelectedOrderItem{ get; private set; }

		/// <summary>
		/// Removes an item - void if it's already sent, delete otherwise
		/// </summary>
		/// <value>The void or delete order item.</value>
		public ICommand DeleteOrderItem{ get; private set; }

		private void DoDeleteOrderItem()
		{
			if (Model.SelectedOrderItem != null) {
				if (Model.SelectedOrderItem.Status == OrderItemStatus.Added) {
					Model.DeleteSelectedOrderItem();
					//load our order items
					if (OnLoadOrderItems != null) {
						OnLoadOrderItems(this);
					}
					//PopulateOrderItems (OrderItemsCol);
					if (OnModifierModeChanged != null) {
						OnModifierModeChanged(this);
					}
					if (OnLoadHotItems != null) {
						OnLoadHotItems(this);
					}
					if (OnUpdateCommands != null) {
						OnUpdateCommands(this);
					}
				}
			}
		}

		public OrderOnCheckViewModel(ILogger logger, ITerminalApplicationModel model) : base(logger, model)
		{
			FastCash = new Command(
				() => {
				}, 
				() => Model.SelectedCheck != null && Model.SelectedCheck.Total != null && Model.SelectedCheck.Total.HasValue);

//			Close = new Command(
//				() => {
//					var view = Model.CloseOrderButtonClicked();
//					DestinationView = view;
//
////					MoveToView(view);
//				}, 
//				() => Model.SelectedCheck != null
//				&&
//				((Model.SelectedCheck.Total != null && Model.SelectedCheck.Total.HasValue)
//				|| (Model.SelectedCheck.OrderItems.Any())
//				)
//			);

			Cancel = new Command(
				() => {
					Model.CancelOrdering();
					//TODO change this to move back to where we came from instead
//					MoveToView(TerminalViewTypes.ViewChecks);
				});

			Send = new Command(() => {
				//TODO make this async to create faster?
				var finishCommit = Model.SendSelectedCheckAsync();
//				MoveToView(TerminalViewTypes.ViewChecks);
				finishCommit.Wait();
			});

			Extra = new Command(() => {
			});

			MenuItemClicked = new Command<Mise.Core.Entities.Menu.MenuItem>(
				item => {
					try {
						var orderItem = Model.DrinkClicked(item);
						if (Model.CurrentTerminalViewTypeToDisplay == TerminalViewTypes.ModifyOrder) {
							//we need a modifier to cotinue
							ModifierActive = true;
							CurrentlyOrdering = true;
							if (OnLoadModifiers != null) {
								OnLoadModifiers(this);
							}
						} else {
							Model.OrderItemOrderingCompleted(orderItem);
							CurrentlyOrdering = false;
							if (OnLoadCategories != null) {
								OnLoadCategories(this);
							}
							if (OnLoadMenuItems != null) {
								OnLoadMenuItems(this);
							}
						}

						if (OnLoadOrderItems != null) {
							OnLoadOrderItems(this);
						}
						if (OnLoadHotItems != null) {
							OnLoadHotItems(this);
						}
						if (OnUpdateCommands != null) {
							OnUpdateCommands(this);
						}
					} catch (Exception e) {
						Logger.HandleException(e);
					} 
				}
			);

			CategoryClicked = new Command<MenuItemCategory>(category => {
				//are we reclicking our currently selected one?
				Model.SelectedCategory = category;
				if (OnLoadCategories != null) {
					OnLoadCategories(this);
				}

				if (OnLoadMenuItems != null) {
					OnLoadMenuItems(this);
				}

				if (OnLoadHotItems != null) {
					OnLoadHotItems(this);
				}

				if (OnUpdateCommands != null) {
					OnUpdateCommands(this);
				}
			});

			CategoryUpClicked = new Command(() => {
				//find our parent category
				Model.SelectedCategory = Model.SelectedCategoryParent;

				if (OnLoadCategories != null) {
					OnLoadCategories(this);
				}
				if (OnLoadMenuItems != null) {
					OnLoadMenuItems(this);
				}
				if (OnLoadHotItems != null) {
					OnLoadHotItems(this);
				}

				if (OnUpdateCommands != null) {
					OnUpdateCommands(this);
				}
			},
				() => Model.SelectedCategory != null
			);

			CategoryHomeClicked = new Command(
				() => {
					Model.SelectedCategory = null;
					if (OnLoadCategories != null) {
						OnLoadCategories(this);
					}
					if (OnLoadMenuItems != null) {
						OnLoadMenuItems(this);
					}
					if (OnLoadHotItems != null) {
						OnLoadHotItems(this);
					}
					if (OnUpdateCommands != null) {
						OnUpdateCommands(this);
					}
				},
				() => CategoryIsShowing
			);

			DeleteOrderItem = new Command(
				DoDeleteOrderItem, 
				() => Model.SelectedOrderItem != null
			);

			CompSelectedOrderItem = new Command(
				() => {
					var comped = Model.CompSelectedItem();
					if (comped) {
						ModifierActive = false;

						if (OnLoadOrderItems != null) {
							OnLoadOrderItems(this);
						}
						if (OnModifierModeChanged != null) {
							OnModifierModeChanged(this);
						}
					}
				}, 
				() => SelectedOrderItem != null
				&& SelectedOrderItem.IsComped == false
				&& Model.SelectedEmployee != null
				&& Model.SelectedEmployee.CompBudget != null
				&& Model.SelectedEmployee.CompBudget.GreaterThanOrEqualTo(SelectedOrderItem.Total)
			);

			UndoCompedSelectedOrderItem = new Command(
				() => {
					Model.UndoCompOnSelectedOrderItem();
					ModifierActive = false;
					if (OnLoadOrderItems != null) {
						OnLoadOrderItems(this);
					}
					if (OnModifierModeChanged != null) {
						OnModifierModeChanged(this);
					}
				},
				() => SelectedOrderItem != null
				&& SelectedOrderItem.IsComped
				&& SelectedOrderItem.EmployeeWhoComped.HasValue
				&& SelectedOrderItem.EmployeeWhoComped.Value == Model.SelectedEmployee.ID
			);
		}
	}
}

