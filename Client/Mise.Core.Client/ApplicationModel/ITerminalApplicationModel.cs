using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Menu;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;
using System;
using Mise.Core.Client.Services;
using Mise.Core.Services;


namespace Mise.Core.Client.ApplicationModel
{
	/// <summary>
	/// View model that encapsulates all behavior for a terminal (order taking application)
	/// </summary>
	public interface ITerminalApplicationModel
	{
		#region Mise Services
		IPrinterService LocalPrinterService { get; }
		ICreditCardReaderService CreditCardReaderService{get;}
		#endregion

		/// <summary>
		/// Whether we're setup and ready to go
		/// </summary>
		/// <value><c>true</c> if ready; otherwise, <c>false</c>.</value>
		bool Setup{get;}

		/// <summary>
		/// If there's currently any unsaved changes
		/// </summary>
		bool Dirty{ get; }

		/// <summary>
		/// Whether we're online and connected to our service or not
		/// </summary>
		/// <value><c>true</c> if online; otherwise, <c>false</c>.</value>
		bool Online{get;}

        /// <summary>
        /// The last time the view model was updated regarding checks or employees, used to only load screens when needed
        /// </summary>
        DateTime ChecksLastUpdated { get; }

        /// <summary>
        /// Tells us which view the application should be displaying at any time
        /// </summary>
        TerminalViewTypes CurrentTerminalViewTypeToDisplay { get; }

		#region Employees
		/// <summary>
		/// All available employees - might not be accessable for phones
		/// </summary>
		/// <value>The employees.</value>
		IEnumerable<IEmployee> CurrentEmployees{get;}

		/// <summary>
		/// The employee the application is currently dealing with, or null
		/// </summary>
		/// <value>The selected employee.</value>
		IEmployee SelectedEmployee{ get; set;}


		IEmployee LastSelectedEmployee{get;}

		/// <summary>
		/// Fired when we click on an employee name
		/// </summary>
		/// <param name="employee">Employee.</param>
		void EmployeeClicked(IEmployee employee);

		/// <summary>
		/// Clocks an employee in, if the passcode was correct
		/// </summary>
		/// <returns><c>true</c>, if clockin was employeed, <c>false</c> otherwise.</returns>
		/// <param name="passcode">Passcode.</param>
		bool EmployeeClockin (string passcode);
		/// <summary>
		/// Clocks the employee out, if the passcode is correct
		/// </summary>
		/// <returns><c>true</c>, if clockout was employeed, <c>false</c> otherwise.</returns>
		/// <param name="passcode">Passcode.</param>
		/// <param name="employee">Employee.</param>
		bool EmployeeClockout(string passcode, IEmployee employee);

		/// <summary>
		/// Employee attempts to run a no sale event
		/// </summary>
		/// <returns><c>true</c>, if sale was noed, <c>false</c> otherwise.</returns>
		/// <param name="passcode">Passcode.</param>
		bool NoSale (string passcode);
		#endregion

		#region Checks
		/// <summary>
		/// If we're currently in a state where we can open checks
		/// </summary>
		/// <value><c>true</c> if this instance can open checks; otherwise, <c>false</c>.</value>
		bool CanOpenChecks{get;}

		IEnumerable<ICheck> AllChecks{ get; }

		/// <summary>
		/// All Checks available which are not yet marked as closed
		/// </summary>
		/// <value>The Checks.</value>
		IEnumerable<ICheck> OpenChecks{get;}

		IEnumerable<ICheck> ClosedChecks{get;}

		/// <summary> 
		/// The Check we're currently working with
		/// </summary>
		/// <value>The selected Check.</value>
		ICheck SelectedCheck{get;}

		/// <summary>
		/// When a Check is selected
		/// </summary>
		TerminalViewTypes CheckClicked(ICheck check);

		/// <summary>
		/// Create a new Check for use
		/// </summary>
		/// <returns>The Check.</returns>
		ICheck CreateNewCheck (PersonName newName);
		ICheck CreateNewCheck(CreditCard card);

		/// <summary>
		/// From terminal settings.  If true, anyone can add to any check without having to be logged in or keep it.  Otherwise, you need to be logged in
		/// </summary>
		/// <value><c>true</c> if this instance can modify checks server doesnt own; otherwise, <c>false</c>.</value>
		bool CanModifyChecksServerDoesntOwn{ get; }

		#endregion

		#region CashDrawer
		/// <summary>
		/// Lets the views set an action to do when the drawer is closed.  We'll set the next view internally,
		/// so the view just needs to do their UI stuff
		/// </summary>
		/// <param name="toDoWhenClosed">To do when closed.</param>
		void SetCashDrawerClosedEvent (Action toDoWhenClosed);

		void OpenCashDrawer();

		/// <summary>
		/// Whether or not this terminal has a cash drawer on it
		/// </summary>
		bool HasCashDrawer{get;}
		#endregion

		#region Drinks


		/// <summary>
		/// Category we've currently selected
		/// </summary>
		/// <value>The current category.</value>
		IEnumerable<MenuItemCategory> Categories{ get; }

		/// <summary>
		/// Retrive the category the menu item is a part of
		/// </summary>
		/// <returns>The category for item.</returns>
		/// <param name="menuItem">Menu item.</param>
		MenuItemCategory GetCategoryForItem (MenuItem menuItem);

		MenuItemCategory GetCategoryByName (string name);

        /// <summary>
        /// Category we've currently selected
        /// </summary>
        /// <value>The current category.</value>
		MenuItemCategory SelectedCategory { get; set;}

		/// <summary>
		/// Category our current one belongs to.  If top, it's null
		/// </summary>
		/// <value>The selected category parent.</value>
		MenuItemCategory SelectedCategoryParent{ get; }

		/// <summary>
		/// The current sub categories
		/// </summary>
		/// <value>The current categories.</value>
		IEnumerable<MenuItemCategory> CurrentSubCategories{get;}

		/// <summary>
		/// Given a menu item, find out which subcategory under the current one displaying it should go to
		/// </summary>
		/// <returns>The currently displayed category for item.</returns>
		/// <param name="item">Item.</param>
		MenuItemCategory GetCurrentlyDisplayedCategoryForItem (MenuItem item);


		/// <summary>
		/// All menu items under this category and any of its subcategories
		/// </summary>
		/// <value>The menu items under current category.</value>
		IEnumerable<MenuItem> MenuItemsUnderCurrentCategory{ get; }

		/// <summary>
		/// Gets our categories and a list of items to display for them
		/// </summary>
		/// <returns>The category names and menu items.</returns>
		/// <param name="maxItemsOnScreen">Max items on screen.</param>
		/// <param name="maxItemsInRow">Max items in row.</param>
		IEnumerable<Tuple<MenuItemCategory, IEnumerable<MenuItem>>> GetCategoryNamesAndMenuItems (int maxItemsOnScreen, int? maxItemsInRow);

		IEnumerable<MenuItem> HotItemsUnderCurrentCategory{get;}

		IEnumerable<MenuItem> MiseItems{get;}

		/// <summary>
		/// When a drink is clicked, turn it into an order item
		/// </summary>
		/// <param name="drink">Drink.</param>
		OrderItem  DrinkClicked (MenuItem drink);

		/// <summary>
		/// When ordering an item, signals that we've completed all our required modifiers and the like
		/// </summary>
		/// <param name="orderItem">Order item.</param>
		void OrderItemOrderingCompleted (OrderItem  orderItem);
		#endregion


		#region Ordered Drinks
		//drinks are under the selected category

		/// <summary>
		/// Ordereds the drink clicked.
		/// </summary>
		/// <param name="drink">Drink.</param>
		void OrderedDrinkClicked (OrderItem  drink);

		/// <summary>
		/// OrderItem selected for modifications
		/// </summary>
		/// <value>The selected order item.</value>
		OrderItem  SelectedOrderItem{ get;}

		/// <summary>
		/// Remove an item that was entered, but not yet sent
		/// </summary>
		bool DeleteSelectedOrderItem ();

		/// <summary>
		/// Voids the selected order item.
		/// </summary>
		/// <param name="managerPasscode">Manager passcode.</param>
		/// <param name="reason">the reason for the void, will be sent on the event</param>
		bool VoidSelectedOrderItem (string managerPasscode, string reason, bool waste);

		/// <summary>
		/// Adds an identical copy of the selected order item to the check
		/// </summary>
		OrderItem  ReorderSelectedOrderItem();
		#endregion

		#region Modifiers

		/// <summary>
		/// Marks an OI as selected, and enters the modification mode
		/// </summary>
		/// <param name="oi">Oi.</param>
		void SelectOrderItemToModify (OrderItem  oi);

		/// <summary>
		/// Finishes modification mode, applying 0-many modifiers, and optionally multiplying the order
		/// </summary>
		/// <param name="modifiers">Modifiers.</param>
		/// <returns>true if modified, false if not (missing required, etc)</returns>
		bool ModifySelectedOrderItem (IList<MenuItemModifier> modifiers);

		bool SetMemoOnSelectedOrderItem (string memo);

		/// <summary>
		/// If we started to modify then cancelled
		/// </summary>
		/// <returns><c>true</c> if this instance cancel modification on selected order item; otherwise, <c>false</c>.</returns>
		void CancelModificationOnSelectedOrderItem();

		/// <summary>
		/// Give a menu item and modes, calculate the modified price
		/// </summary>
		/// <returns>The new running price for mod.</returns>
		/// <param name="item">Item.</param>
		/// <param name="mods">Mods.</param>
		Money CalculateNewRunningPriceForMod (MenuItem item, IEnumerable<MenuItemModifier> mods);
		#endregion

        #region Payment
		/// <summary>
		/// Find how much the sales tax on our current check is
		/// </summary>
		/// <returns>The sales tax for selected check.</returns>
		Money GetSalesTaxForSelectedCheck ();

		Money GetTotalWithSalesTaxForSelectedCheck();

		/// <summary>
		/// Shows us how much money is to be paid to close a check
		/// </summary>
		/// <returns>The remaining amount to be paid on check.</returns>
		Money GetRemainingAmountToBePaidOnCheck ();

		/// <summary>
		/// If true, we run our own credit cards through mise.  If not, we use an outside term
		/// </summary>
		bool UseIntegratedCredit{ get; }

		/// <summary>
		/// Cancel any payments we made on this screen
		/// </summary>
		/// <returns><c>true</c> if this instance cancel payments; otherwise, <c>false</c>.</returns>
		void CancelPayments();

		/// <summary>
		/// We want to save the payments we have, but not 
		/// </summary>
		bool SavePaymentsClicked ();

		bool PayCheckWithCash(Money amountTendered);

		/// <summary>
		/// Put a comp amount on the check
		/// </summary>
		/// <param name="amountToComp">Amount to comp.</param>
		/// <returns>>True if the employee can do the comp, false otherwise</returns>
		bool PayCheckWithAmountComp (Money amountToComp);

		/// <summary>
		/// Gets an authorization for a credit card and add a payment.  Our service will 
		/// take care of passing it through it's paces.  This moves it to get Authorized
		/// </summary>
		/// <returns><c>true</c>, if check with credit card was paid, <c>false</c> otherwise.</returns>
		/// <param name="card">Card.</param>
		/// <param name="amt">Amt.</param>
		bool PayCheckWithCreditCard (CreditCard card, Money amt);

		IEnumerable<IProcessingPayment> WaitingForTipPayments{get;}

		/// <summary>
		/// Given a processing payment that has come back authorized, finish it up (close or put to closing batch)
		/// </summary>
		/// <returns><c>true</c>, if processing payment was closed, <c>false</c> otherwise.</returns>
		/// <param name="payment">Payment to close, that is in base authorized</param>
		/// <param name="tipAmount">Tip amount.</param>
		bool AddTipToProcessingPayment (IProcessingPayment payment, Money tipAmount);

		/// <summary>
		/// For a processing payment that has it's base amount authorized, cancel it!
		/// </summary>
		/// <param name="payment">Payment.</param>
		bool VoidAuthorizedProcessingPayment (IProcessingPayment payment);

		/// <summary>
		/// Pay the check via an external credit system
		/// </summary>
		/// <param name = "amount">Amount on the slip</param>
		/// <param name="tipAmount">Tip amount on the slip.</param>
		void AddExternalCreditCardPayment (Money amount, Money tipAmount);

		/// <summary>
		/// Reopens the selected Check so it can be ordered on
		/// </summary>
		/// <returns>The selected Check.</returns>
		ICheck ReopenSelectedCheck();

		/// <summary>
		/// If our payments are full up, mark the check as paid and close.  
		/// If not, return null
		/// </summary>
		/// <returns>Updated check, or null if not able to mark it</returns>
		ICheck MarkSelectedCheckAsPaid ();

		/// <summary>
		/// Sets a function that will be called when a credit card is done processing
		/// with that
		/// </summary>
		/// <param name="callback">Callback.  Gives the check that was processed on it</param>
		void SetCreditCardProcessedCallback (Action<ICheck> callback);

		/// <summary>
		/// Comp the selected item from the current employee's bucket
		/// </summary>
		/// <returns>if rejected, false</returns>
		bool CompSelectedItem();

		void UndoCompOnSelectedOrderItem ();


		/// <summary>
		/// Get 15%, etc
		/// </summary>
		/// <returns>The typical tip amounts on selected check.</returns>
		IEnumerable<Money> GetTypicalTipAmountsOnSelectedCheck(int numToGet);


		#region Discounts
		/// <summary>
		/// Get any possible discounts for display
		/// </summary>
		/// <returns>The possible discounts.</returns>
		IEnumerable<IDiscount> GetPossibleDiscounts ();
		/// <summary>
		/// Get any possible gratuities (add ons)
		/// </summary>
		/// <returns>The possible gratuities.</returns>
		IEnumerable<IDiscount> GetPossibleGratuities ();

		/// <summary>
		/// Take some gratuities and discounts, and apply them to the current check
		/// </summary>
		/// <returns>The discounts to selected check.</returns>
		/// <param name="discounts">Discounts.</param>
		ICheck AddDiscountsToSelectedCheck (IEnumerable<IDiscount> discounts);

		ICheck RemoveDiscountsFromSelectedCheck (IEnumerable<IDiscount> discounts);
		#endregion
	    #endregion


	    void SendSelectedCheck();

        /// <summary>
        /// Send the Check, but put any long running things into the returned Task. 
        /// This allows the 
        /// </summary>
	    Task SendSelectedCheckAsync();

        /// <summary>
        /// Triggers the view model to remove any unsaved changes
        /// </summary>
	    void CancelOrdering();


	    /// <summary>
	    /// If true, the employee needs to put their passcode in to order
	    /// </summary>
	    bool RequireEmployeeSignIn { get; }

		/// <summary>
		/// Fired when the cash button is clicked
		/// </summary>
	    TerminalViewTypes CloseOrderButtonClicked();

		void GoToHomeScreen ();
	}
}

