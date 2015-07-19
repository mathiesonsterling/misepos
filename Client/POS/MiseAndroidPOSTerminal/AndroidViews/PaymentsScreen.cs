using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Text;
using Android.Views.InputMethods;
using Mise.Core.Entities.Check;
using Mise.Core.ValueItems;
using Mise.Core.Client.ApplicationModel;
using Mise.Core.Entities.Payments;
using MiseAndroidPOSTerminal.AndroidControls;
using MisePOSTerminal.ViewModels;


namespace MiseAndroidPOSTerminal.AndroidViews
{
	[Activity (Label = "Payments", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation)]
	public class PaymentsScreen : BasePOSViewWithOrderItemList
	{
		PaymentsViewModel _vm;

		LinearLayout _dueRows;
		EditText _amountEdit;
		TextView _amountDue;

		LinearLayout _commands;
		LinearLayout _paymentSpecificButtonArea;
		LinearLayout _payments;
		LinearLayout _tipPaymentsArea;
		LinearLayout _centerScrollHolder;
		Button _reopenButton;
		Button _payButton;
		Button _saveButton;

		Button _creditButton;
		Button _compButton;
		Button _cashButton;
		Button _addGratButton;

		TextView _ccNameLabel;
		TextView _ccNumberLabel;

		EditText _tipEdit;
		#region Android events

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			_vm = new PaymentsViewModel (Logger, Model);
			_vm.OnMoveToView += calling => MoveToView (calling.DestinationView);
			_vm.OnUpdatePayments += calling => PopulatePayments();
			_vm.OnLoadPaymentForTipAmount += (calling) => {
				var vm = calling as PaymentsViewModel;
				var payment = vm.SelectedPayment as ICreditCardPayment;
				_amountDue.Text = payment.AmountToApplyToCheckTotal.ToFormattedString ();

				//load the name, number
				_ccNameLabel.Text = payment.Card.FirstName + " " + payment.Card.LastName;
				_ccNumberLabel.Text = "**** **** ****" + payment.Card.CardNumber.Substring (11);

				//clean out our edit as well
				_tipEdit.Text = vm.TipAmount.HasValue ? new Money(vm.TipAmount.Value).ToFormattedString () : string.Empty;
			};
			_vm.OnCommandsUpdateStatus += UpdateCommandsStatus;
			_vm.OnUpdateSelectedPaymentType += calling => SelectPaymentType ();

			_vm.PropertyChanged += (sender, e) => {
				switch (e.PropertyName)
				{
				case "CheckTotal":
					break;
				case "AmountStillDue":
					_amountDue.Text = _vm.AmountStillDue.ToString("C");
					break;
				case "AmountToPay":
					_amountEdit.Text = _vm.AmountToPay.ToString("C");
					break;
				}
			};


			//Create the user interface in code
			var mainLayout = new LinearLayout (this){ Orientation = Orientation.Vertical };
			var layout = new LinearLayout (this) {Orientation = Orientation.Horizontal};
			mainLayout.AddView (layout);

			//main sections
			var leftSideCol = new LinearLayout (this) { Orientation = Orientation.Vertical };
			leftSideCol.SetMinimumWidth (POSTheme.OrderItemPanelWidth);
			layout.AddView (leftSideCol);

			var oiWrapper = CreateOrderItemArea ();
			leftSideCol.AddView (oiWrapper);


			var middleCol = new LinearLayout (this){ Orientation = Orientation.Vertical };
			layout.AddView (middleCol);

			//add the total due, and the payment edit button
			_dueRows = CreateAmountRows ();
			middleCol.AddView (_dueRows);

			_centerScrollHolder = new LinearLayout (this){DividerPadding=0};
			_centerScrollHolder.SetPadding (0, 0, 0, 0);
			_centerScrollHolder.SetMinimumWidth (POSTheme.PaymentsScreenMiddleColumnWidth);
			var centerScrollerLayoutParams = new LinearLayout.LayoutParams (POSTheme.PaymentsScreenMiddleColumnWidth, POSTheme.PaymentsPaymentSpecificAreaScrollerHeight);
			centerScrollerLayoutParams.BottomMargin = 0;
			_centerScrollHolder.LayoutParameters = centerScrollerLayoutParams;
			middleCol.AddView (_centerScrollHolder);

			var paymentButtonAreaScroller = new ScrollView (this);
			paymentButtonAreaScroller.SetPadding (0, 0, 0, 0);
			_centerScrollHolder.AddView (paymentButtonAreaScroller);

			_paymentSpecificButtonArea = new LinearLayout (this) { Orientation = Orientation.Vertical };
			_paymentSpecificButtonArea.SetMinimumWidth (POSTheme.PaymentsScreenMiddleColumnWidth);
			_paymentSpecificButtonArea.SetMinimumHeight (POSTheme.PaymentsPaymentSpecificAreaScrollerHeight);
			paymentButtonAreaScroller.AddView (_paymentSpecificButtonArea);

			_tipPaymentsArea = CreateTipRows ();
			middleCol.AddView (_tipPaymentsArea);

			//add our payments stuff here
			var paymentsScrollHolder = new LinearLayout (this){ DividerPadding = 0 };
			paymentsScrollHolder.SetPadding (0, 0, 0, 0);
			var paymentsScrollHolderLayoutParams = new LinearLayout.LayoutParams (POSTheme.PaymentsScreenMiddleColumnWidth, POSTheme.PaymentsPaymentListScrollerHeight);
			paymentsScrollHolderLayoutParams.BottomMargin = 0;
			paymentsScrollHolder.LayoutParameters = paymentsScrollHolderLayoutParams;
			middleCol.AddView (paymentsScrollHolder);

			var paymentScroller = new ScrollView (this);
			paymentScroller.SetPadding (0, 0, 0, 0);
			paymentsScrollHolder.AddView (paymentScroller);

			_payments = new LinearLayout (this){ Orientation = Orientation.Vertical };
			paymentScroller.AddView (_payments);

			var rightCol = new LinearLayout (this) { Orientation = Orientation.Vertical };
			rightCol.SetMinimumWidth (POSTheme.PaymentsScreenRightColumnWidth);
			layout.AddView (rightCol);

			_commands = new LinearLayout (this) { Orientation = Orientation.Vertical };
			rightCol.AddView (_commands);
			CreatePaymentTypeButtons ();

			var commandBar = CreateCommandBar ();
			mainLayout.AddView (commandBar);

			UpdateCommandsStatus (_vm);

			SetContentView (mainLayout);
		}
			

		protected override void OnResume ()
		{
			try{
				base.OnResume ();
				if (Model != null) {
					Model.SetCreditCardProcessedCallback (check => {});
					PopulateOrderItems ();
					//note we don't need to start a transaction here, we already have one from the tab click!
					PopulatePayments ();
					if (Model.CurrentTerminalViewTypeToDisplay == TerminalViewTypes.AddTips) {
						_dueRows.Visibility = ViewStates.Gone;
						_saveButton.Visibility = ViewStates.Gone;
						_centerScrollHolder.Visibility = ViewStates.Gone;
						_paymentSpecificButtonArea.Visibility = ViewStates.Gone;
						_tipPaymentsArea.Visibility = ViewStates.Visible;

						//we've got a ran credit card on this, just go to add tip stuff
						if(_vm.WaitingForTipPayments.Any()){
							//tell the VM to load the amount
							_vm.LoadPaymentForTipAmount ();
						}
					} else {
						_dueRows.Visibility = ViewStates.Visible;
						_saveButton.Visibility = ViewStates.Visible;
						_paymentSpecificButtonArea.Visibility = ViewStates.Visible;
						_centerScrollHolder.Visibility = ViewStates.Visible;
						_tipPaymentsArea.Visibility = ViewStates.Gone;

						_vm.SelectedPaymentType = null;
						SelectPaymentType ();
						//see if we can comp at all
						if(Model.SelectedEmployee.CompBudget == null || (Model.SelectedEmployee.CompBudget.HasValue == false )){
							POSTheme.MarkButtonAsDisabled (_compButton);
						}

						CashButtonClicked(this, null);
					}
				}
			}catch(Exception e){
				Logger.HandleException (e);
			}
		}
			

		#endregion
		void UpdateCommandsStatus(BaseViewModel calling){
			var vm = calling as PaymentsViewModel;
			POSTheme.SetButtonEnabled (_reopenButton, vm.Reopen.CanExecute (null));
			POSTheme.SetButtonEnabled (_saveButton, vm.Save.CanExecute (null));

			_amountDue.Text = _vm.AmountStillDue.ToString("C");
			_amountEdit.Text = _vm.AmountStillDue.ToString("C");

			if (_vm.Pay.CanExecute (null) == false) {
				POSTheme.MarkButtonAsDisabled (_payButton);
				_amountEdit.Enabled = true;
			}
			else {
				//we can close!
				_amountEdit.Enabled = false;
				POSTheme.MarkButtonAsEnabled (_payButton);
			}
		}

		#region Create Screen
		LinearLayout CreateCommandBar(){
			var commandBar = new LinearLayout (this){ Orientation = Orientation.Horizontal };
			_saveButton = POSTheme.CreateButton (this, "Save", POSTheme.PaymentsSentButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			_saveButton.SetMinHeight (POSTheme.OrdersScreenCommandBarHeight);
			_saveButton.SetMinWidth (POSTheme.CommandButtonWidth);
			_saveButton.Click += (sender, e) => _vm.Save.Execute (null);
			commandBar.AddView (_saveButton);

			var cancelButton = POSTheme.CreateButton (this, "Cancel", POSTheme.CancelButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			cancelButton.SetMinimumHeight (POSTheme.OrdersScreenCommandBarHeight);
			cancelButton.SetMinimumWidth (POSTheme.CommandButtonWidth);
			cancelButton.Click += (sender, e) => _vm.Cancel.Execute (null);
			commandBar.AddView (cancelButton);

			_reopenButton = POSTheme.CreateButton (this, "Reopen", POSTheme.ReopenButtonColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			_reopenButton.SetMinimumHeight (POSTheme.OrdersScreenCommandBarHeight);
			_reopenButton.SetMinimumWidth (POSTheme.CommandButtonWidth);
			_reopenButton.Click += (sender, e) => _vm.Reopen.Execute (null);
			commandBar.AddView (_reopenButton);

			_payButton = POSTheme.CreateButton (this, "Pay Check", POSTheme.PayCheckColor, POSTheme.CommandButtonWidth, POSTheme.OrdersScreenCommandBarHeight, 0, 0);
			_payButton.SetMinHeight (POSTheme.OrdersScreenCommandBarHeight);
			_payButton.SetMinimumWidth (POSTheme.CommandButtonWidth);
			_payButton.Click += (sender, e) => {
				if (_vm.Pay.CanExecute (null)) {
					_vm.Pay.Execute (null);
				}
			};

			commandBar.AddView (_payButton);

			return commandBar;
		}
		/// <summary>
		/// Load the buttons that appear when we first load
		/// </summary>
		void CreatePaymentTypeButtons ()
		{
			_commands.RemoveAllViews();

			_cashButton = POSTheme.CreateButton (this, "Cash", POSTheme.CashButtonColor);
			_cashButton.SetMinimumWidth (POSTheme.PaymentsScreenRightColumnWidth);
			_cashButton.SetMinimumHeight (POSTheme.CashButtonHeight);
			_cashButton.Click += CashButtonClicked;
			POSTheme.SetButtonEnabled (_cashButton, _vm.CanTakeCash);
			_commands.AddView (_cashButton);

			_creditButton = POSTheme.CreateButton (this, "Credit", POSTheme.CreditButtonColor);
			_creditButton.SetMinimumWidth (POSTheme.PaymentsScreenRightColumnWidth);
			_creditButton.SetMinimumHeight (POSTheme.CreditButtonHeight);
			_creditButton.Click += CreditButtonClicked;
			_commands.AddView (_creditButton);

			if (_vm.CanCompAmounts) {
				_compButton = POSTheme.CreateButton (this, "Comp", POSTheme.CompColor);
				_compButton.SetMinimumWidth (POSTheme.PaymentsScreenRightColumnWidth);
				_compButton.SetMinimumHeight (POSTheme.AddGratuityHeight);
				_compButton.Click += CompButtonClicked;
				_commands.AddView (_compButton);
			}

			_addGratButton = POSTheme.CreateButton (this, "Discounts/Gratuity", POSTheme.AddGratuityColor);
			_addGratButton.SetMinimumWidth (POSTheme.PaymentsScreenRightColumnWidth);
			_addGratButton.SetMinHeight (POSTheme.AddGratuityHeight);
			_addGratButton.Click += GratuityAndDiscountsClicked;
			_commands.AddView (_addGratButton);


		}
			
		LinearLayout CreateTipRows(){
			var centerScrollHolder = new LinearLayout (this){DividerPadding=0};
			centerScrollHolder.SetPadding (0, 0, 0, 0);
			centerScrollHolder.SetMinimumWidth (POSTheme.PaymentsScreenMiddleColumnWidth);
			var centerScrollerLayoutParams = new LinearLayout.LayoutParams (POSTheme.PaymentsScreenMiddleColumnWidth, POSTheme.PaymentsPaymentSpecificAreaScrollerHeight);
			centerScrollerLayoutParams.BottomMargin = 0;
			centerScrollHolder.LayoutParameters = centerScrollerLayoutParams;

			var tipRowContainer = new LinearLayout (this) { Orientation = Orientation.Vertical };
			tipRowContainer.SetMinimumWidth (POSTheme.PaymentsScreenMiddleColumnWidth);
			tipRowContainer.SetMinimumHeight (POSTheme.PaymentsScreenUpperMiddleHeight);
			tipRowContainer.Visibility = ViewStates.Gone;
			//centerScrollHolder.AddView (tipRowContainer);

			var amountPaid = _vm.CheckTotal;
			var dueRow = new LinearLayout (this) {
				Orientation = Orientation.Horizontal
			};

			var amountDueL = new TextView (this) {
				Text = "Amount Paid: "
			};
			amountDueL.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			dueRow.AddView (amountDueL);
			_amountDue = new TextView (this) {
				Text = amountPaid.ToString ("C")
			};
			_amountDue.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			dueRow.AddView (_amountDue);
			tipRowContainer.AddView (dueRow);

			//add the credit card name and number (last 4 only) fields
			var nameRow = POSTheme.CreateRowForGrid (this);
			_ccNameLabel = new TextView (this);
			_ccNameLabel.SetTextColor (POSTheme.DefaultTextColor);
			_ccNameLabel.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentTextSize);
			nameRow.AddView (_ccNameLabel);
			tipRowContainer.AddView (nameRow);

			var numberRow = POSTheme.CreateRowForGrid (this);
			tipRowContainer.AddView (numberRow);
			_ccNumberLabel = new TextView (this);
			_ccNumberLabel.SetTextColor (POSTheme.DefaultTextColor);
			_ccNumberLabel.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentTextSize);
			numberRow.AddView (_ccNumberLabel);

			//add the edit area
			var tipRow = POSTheme.CreateRowForGrid (this);
			tipRowContainer.AddView (tipRow);

			var tipLabel = new TextView (this){Text = "Tip:  "};
			tipLabel.SetTextColor (POSTheme.DefaultTextColor);
			tipLabel.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentTextSize);
			tipRow.AddView (tipLabel);

			var addTipButton = POSTheme.CreateButton (this, "Add Tip", POSTheme.AddTipColor);

			_tipEdit = new EditText (this){InputType = InputTypes.NumberFlagDecimal | InputTypes.ClassNumber};
			_tipEdit.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			_tipEdit.SetTextColor (POSTheme.DefaultTextColor);
			_tipEdit.Text = string.Empty;
			_tipEdit.EditorAction += (sender, e) => {
				var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
				if (inputMgr != null) {
					inputMgr.HideSoftInputFromWindow (_tipEdit.WindowToken, HideSoftInputFlags.None);
				}

				//try to parse it
				decimal amount;
				var cleanedTipAmount = _tipEdit.Text.Trim().Replace("$", "");
				if (decimal.TryParse (cleanedTipAmount, out amount)) {
					var money = new Money(amount);
					_vm.TipAmount = amount;
					_tipEdit.Text = money.ToFormattedString();
				} else {
					_vm.TipAmount = null;
					_tipEdit.Text = Money.None.ToFormattedString();
				}

				var enabled = _vm.AddTip.CanExecute (null);
				POSTheme.SetButtonEnabled (addTipButton, enabled);
			};
			tipRow.AddView (_tipEdit);

			//add Add Tip and Void Auth buttons
			var btnRow = POSTheme.CreateRowForGrid (this);
			tipRowContainer.AddView (btnRow);

			addTipButton.SetMinimumWidth(POSTheme.PaymentItemButtonWidth);
			addTipButton.SetMinimumHeight (POSTheme.PayCheckHeight);
			addTipButton.Click += (sender, e) => {
				decimal amount;
				var cleanedTipAmount = _tipEdit.Text.Trim().Replace("$", "");
				if (decimal.TryParse (cleanedTipAmount, out amount)) {
					_vm.TipAmount = amount;
					_vm.AddTip.Execute(null);
					_tipEdit.Text = _vm.TipAmount.HasValue? new Money(_vm.TipAmount.Value).ToFormattedString() : string.Empty;
				}
			};
			POSTheme.MarkButtonAsDisabled (addTipButton);
			btnRow.AddView (addTipButton);

			var voidAuthButton = POSTheme.CreateButton (this, "Void Auth", POSTheme.CancelButtonColor);
			voidAuthButton.SetMinimumWidth(POSTheme.PaymentItemButtonWidth);
			voidAuthButton.SetMinimumHeight (POSTheme.PayCheckHeight);
			voidAuthButton.Click += (sender, e) =>{
				if(_vm.VoidAuthorization != null && _vm.VoidAuthorization.CanExecute(null)){
					_vm.VoidAuthorization.Execute (null);
				}
			};
			btnRow.AddView (voidAuthButton);


			return tipRowContainer;
			//return centerScrollHolder;
		}

		/// <summary>
		/// Creates the amount due and editor fields for all payment types
		/// </summary>
		/// <returns>The amount rows.</returns>
		LinearLayout CreateAmountRows(){
			var amtRowContainer = new LinearLayout (this){ Orientation = Orientation.Vertical };
			//Add the display of the amount remaining
			var dueRow = new LinearLayout (this) {
				Orientation = Orientation.Horizontal
			};

			var amountDueL = new TextView (this) {
				Text = "Due: "
			};
			amountDueL.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			dueRow.AddView (amountDueL);
			_amountDue = new TextView (this) {
				Text = _vm.AmountStillDue.ToString("C")
			};
			_amountDue.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			dueRow.AddView (_amountDue);
			amtRowContainer.AddView (dueRow);

			var amountRow = new LinearLayout (this) {
				Orientation = Orientation.Horizontal
			};
			var amountLabel = new TextView (this) {
				Text = "Tendered:       "
			};
			amountLabel.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			amountRow.AddView (amountLabel);
			_amountEdit = new EditText (this) {
				InputType = InputTypes.NumberFlagDecimal | InputTypes.ClassNumber
			};
			_amountEdit.SetTextSize (ComplexUnitType.Pt, POSTheme.PaymentsAmountTextSize);
			_amountEdit.EditorAction += AmountEditChanged;
			_amountEdit.ClearFocus ();
			amountRow.AddView (_amountEdit);

			amtRowContainer.AddView (amountRow);

			return amtRowContainer;
		}

		TextView CreateCreditCardTextView(string text){
			var tv = new TextView (this) { Text = text};
			tv.SetTextSize (ComplexUnitType.Pt, POSTheme.CreditCardTextSize);
			tv.SetTextColor (POSTheme.CreditCardTextColor);
			tv.Gravity = GravityFlags.Center | GravityFlags.CenterHorizontal;

			return tv;
		}
		#endregion

		/// <summary>
		/// Updates our screen to match the payment type we're on
		/// </summary>
		void SelectPaymentType(){
			//update the type to show it's selected
			_addGratButton.SetBackgroundColor (POSTheme.AddGratuityColor);
			_addGratButton.SetTextColor (POSTheme.DefaultTextColor);

			if(_vm.SelectedPaymentType == PaymentType.Cash){
				POSTheme.MarkAsSelected (_cashButton);
			} else {
				_cashButton.SetBackgroundColor (POSTheme.CashButtonColor);
				_cashButton.SetTextColor (POSTheme.DefaultTextColor);
			}

			if (_compButton != null) {
				if (_vm.SelectedPaymentType == PaymentType.CompAmount) {
					POSTheme.MarkAsSelected (_compButton);
				} else {
					_compButton.SetBackgroundColor (POSTheme.CompColor);
					_compButton.SetTextColor (POSTheme.DefaultTextColor);
				}
			}

			if(_vm.SelectedPaymentType == PaymentType.InternalCreditCard){
				POSTheme.MarkAsSelected (_creditButton);
			} else {
				_creditButton.SetBackgroundColor (POSTheme.CreditButtonColor);
				_creditButton.SetTextColor (POSTheme.DefaultTextColor);
			}

			if(_vm.SelectedPaymentType.HasValue == false){
				_amountEdit.Enabled = false;
				_paymentSpecificButtonArea.RemoveAllViews ();
			} else {
				_amountEdit.Enabled = true;
			}
		}
			

		void PopulateOrderItems(){
			EventHandler handler = (sender, e) => {};
			PopulateOrderItemsList(Model, OrderItemsCol, handler);
		}
			
		void PaymentRowClicked (object sender, EventArgs e)
		{
			if(_vm.InTipMode){
				//select us if we're a credit card
				var payRow = sender as PaymentRow;
				if(payRow != null && payRow.Payment != null){
					var processPayment = payRow.Payment as IProcessingPayment;
					if(processPayment != null){
						//get the check info, load it into the add tip item
					}
				}
			}
		}
			

		/// <summary>
		/// Loads the payments on the check into our bottom frame
		/// </summary>
		void PopulatePayments(){
			_payments.RemoveAllViews ();

			foreach (var payment in _vm.Payments) {
				//create a row
				var payRow = new PaymentRow (this, POSTheme, payment);
				payRow.Click += PaymentRowClicked;

				_payments.AddView (payRow);
			}
		}
			

		//validate the amount is correct for the type, and then add a payment
		//also check if we're now able to pay the check
		void AmountEditChanged (object sender, TextView.EditorActionEventArgs e)
		{
			try{
				var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
				if (inputMgr != null) {
					inputMgr.HideSoftInputFromWindow (_amountEdit.WindowToken, HideSoftInputFlags.None);
				}
					
				//see if we've got a valid amount
				//if the amount edit is an invalid amount
				bool hasError = false;
				Money amtIn = null;
				//switch based on our current type, to validate specifically
				if(_vm.SelectedPaymentType.HasValue){
					decimal amtGiven;
					if (decimal.TryParse (_amountEdit.Text.Replace ('$', ' ').Trim (), out amtGiven)) {
						amtIn = new Money (amtGiven);
						switch(_vm.SelectedPaymentType.Value){
						case PaymentType.CompAmount:
							if(_vm.MakeCompPayment.CanExecute(amtIn)){
								_vm.MakeCompPayment.Execute(amtIn);
							} else {
								hasError = true;
							}
							break;
						case PaymentType.InternalCreditCard:
							if(_vm.MakeInternalCreditPayment.CanExecute(amtIn)){
								_vm.MakeInternalCreditPayment.Execute(amtIn);
							} else {
								hasError = true;
							}
							break;
						case PaymentType.Cash:
							if(_vm.MakeCashPayment.CanExecute(amtIn)){
								_vm.MakeCashPayment.Execute(amtIn);
							} else {
								hasError = true;
							}
							break;
						default:
							throw new Exception("Unhandled payment state given!");
						}
					} else{
						hasError = true;
					}

					if(hasError){
						_amountEdit.SetTextColor (POSTheme.CancelButtonColor); 
					} else {
						_amountEdit.SetTextColor (POSTheme.DefaultTextColor);
					}
				}
			} catch(Exception ex){
				Logger.HandleException (ex);
			}
		}	

		EditText _month;
		EditText _year;
		EditText _numberField;
		Button _doChargeButton;

		void UpdateCreditButtonEnabled(object sender, EventArgs e){
			//TODO move this to pull off the VM's command instead
			decimal amtGiven;
			int monthVal;
			int yearVal;
			if(decimal.TryParse (_amountEdit.Text.Replace ('$', ' ').Trim (), out amtGiven))
			{
				if(int.TryParse (_month.Text, out monthVal)){
					if(int.TryParse (_year.Text, out yearVal)){
						_vm.CreditCardNumber = _numberField.Text;
						_vm.CreditCardMonth = monthVal;
						_vm.CreditCardYear = yearVal;
						var canCharge = _vm.MakeInternalCreditPayment.CanExecute (new Money (amtGiven));
						POSTheme.SetButtonEnabled (_doChargeButton, canCharge);
						if(canCharge){
							var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
							inputMgr.HideSoftInputFromWindow (_numberField.WindowToken, HideSoftInputFlags.None);
							inputMgr.HideSoftInputFromWindow (_month.WindowToken, HideSoftInputFlags.None);
							inputMgr.HideSoftInputFromWindow (_year.WindowToken, HideSoftInputFlags.None);
							inputMgr.HideSoftInputFromWindow (_amountEdit.WindowToken, HideSoftInputFlags.None);
							return;
						}
					}
				}
			}
			POSTheme.MarkButtonAsDisabled (_doChargeButton);
		}

 		void CreditButtonClicked (object sender, EventArgs e)
		{
			try{
				UpdateCommandsStatus (_vm);
				_paymentSpecificButtonArea.RemoveAllViews ();
			
				_vm.SelectedPaymentType = PaymentType.InternalCreditCard;
				SelectPaymentType ();

				var selectedTab = Model.SelectedCheck;
				if (_vm.UseIntegratedCredit) {
					var cardOnTab = selectedTab.CreditCards.FirstOrDefault ();
					//display manual override, but if we get a swipe, populate there!
					var numberRow = POSTheme.CreateRowForGrid (this);
					_paymentSpecificButtonArea.AddView (numberRow);

					var numLabel = CreateCreditCardTextView("Card #:");
					numberRow.AddView (numLabel);

					_numberField = new EditText (this){ InputType = InputTypes.ClassNumber};
					_numberField.ImeOptions = ImeAction.Done;
					_numberField.SetTextColor(POSTheme.CreditCardTextColor);
					_numberField.SetTextSize(ComplexUnitType.Pt,POSTheme.CreditCardTextSize);
					_numberField.SetMinimumWidth (250);
					_numberField.Text = string.Empty;
					_numberField.EditorAction += (senderCN, eCN) => {
						UpdateCreditButtonEnabled (senderCN, eCN);
						//check if our field meets credit card requirements
						//TODO get a better method than just length
						var good = _numberField.Text.Trim ().Length > 14;
						if (good == false) {
							_numberField.Text = string.Empty;
						} else {
							var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
							inputMgr.HideSoftInputFromWindow (_numberField.WindowToken, HideSoftInputFlags.None);
						}
					};
					numberRow.AddView (_numberField);

					var dateRow = POSTheme.CreateRowForGrid (this);
					_paymentSpecificButtonArea.AddView (dateRow);

					var monthLabel = CreateCreditCardTextView("Month:");
					dateRow.AddView (monthLabel);
					_month = new EditText (this){ InputType = InputTypes.ClassNumber};
					_month.ImeOptions = ImeAction.Done;
					_month.SetTextSize (ComplexUnitType.Pt, POSTheme.CreditCardTextSize);
					_month.SetTextColor (POSTheme.CreditCardTextColor);
					_month.EditorAction += UpdateCreditButtonEnabled;
					_month.Text = cardOnTab != null
						? cardOnTab.ExpMonth.ToString() 
							: DateTime.Now.Month.ToString ();
					dateRow.AddView (_month);

					var yearLabel = new TextView (this) { Text = "Year:" };
					yearLabel.SetTextSize (ComplexUnitType.Pt, POSTheme.CreditCardTextSize);
					yearLabel.SetTextColor (POSTheme.CreditCardTextColor);
					dateRow.AddView (yearLabel);
					_year = new EditText (this){ InputType = InputTypes.ClassNumber};
					_year.SetTextSize (ComplexUnitType.Pt, POSTheme.CreditCardTextSize);
					_year.SetTextColor (POSTheme.CreditCardTextColor);
					_year.ImeOptions = ImeAction.Done;
					_year.EditorAction += UpdateCreditButtonEnabled;
					_year.Text = cardOnTab != null
						? cardOnTab.ExpYear.ToString ()
							: DateTime.Now.Year.ToString ();
					dateRow.AddView (_year);

					//add a run card button
					var doChargeRow = POSTheme.CreateRowForGrid (this);
					_paymentSpecificButtonArea.AddView(doChargeRow);

					var cc = POSTheme.CreateButton (this, "CC", POSTheme.CreditButtonColor);
					cc.Click += (object s, EventArgs es) => {
						if (_vm != null) {
							if (_vm.StartCreditCardReader.CanExecute (null)) {
								_vm.StartCreditCardReader.Execute (null);
							}
							//we should probably update the button here to reflect the reading status
							if (_vm.CreditCardReaderReading) {
								//mark it as reading so we know
								cc.SetBackgroundColor (POSTheme.CancelButtonColor);
							} else {
								cc.SetBackgroundColor (POSTheme.CreditButtonColor);
							}
							POSTheme.SetButtonEnabled (cc, _vm.StartCreditCardReader.CanExecute (null));
						}
					};
					POSTheme.SetButtonEnabled (cc, _vm.StartCreditCardReader.CanExecute (null));
					doChargeRow.AddView(cc);

					_doChargeButton = POSTheme.CreateButton (this, "Charge", POSTheme.CreditButtonColor);
					POSTheme.MarkButtonAsDisabled (_doChargeButton);
					_doChargeButton.Click += (object senderCB, EventArgs eCB) => {
						//hide any keyboard that's up
						var inputMgr = GetSystemService (InputMethodService) as InputMethodManager;
						inputMgr.HideSoftInputFromWindow (_numberField.WindowToken, HideSoftInputFlags.None);
						inputMgr.HideSoftInputFromWindow (_month.WindowToken, HideSoftInputFlags.None);
						inputMgr.HideSoftInputFromWindow (_year.WindowToken, HideSoftInputFlags.None);
						inputMgr.HideSoftInputFromWindow (_amountEdit.WindowToken, HideSoftInputFlags.None);

						//if we're a valid amount and card
						var monthVal = 0;
						var yearVal = 0;
						var amtGiven = 0.0M;
						if(decimal.TryParse (_amountEdit.Text.Replace ('$', ' ').Trim (), out amtGiven))
						{
							if(int.TryParse (_month.Text, out monthVal)){
								if(int.TryParse (_year.Text, out yearVal)){
									if( (string.IsNullOrEmpty (_numberField.Text) == false && _numberField.Text.Length > 4)
										&& monthVal > 0 && monthVal < 13 && yearVal >= DateTime.Now.Year){
										_vm.CreditCardNumber = _numberField.Text;
										_vm.CreditCardMonth = monthVal;
										_vm.CreditCardYear = yearVal;

										if(_vm.MakeInternalCreditPayment.CanExecute(new Money(amtGiven))){
											_vm.MakeInternalCreditPayment.Execute(new Money(amtGiven));
											_numberField.Text = _vm.CreditCardNumber;
											_year.Text = _vm.CreditCardYear.ToString();
											_month.Text = _vm.CreditCardMonth.ToString();
										}
									}
					   			}
							}
						}
					};
					doChargeRow.AddView (_doChargeButton);

					var spacer2 = new Space(this);
					spacer2.SetMinimumHeight (POSTheme.PaymentSpaceHeight);
					_paymentSpecificButtonArea.AddView(spacer2);

					var percentAmounts = _vm.PossiblePaymentPercentages.Take(POSTheme.NumberOfCashButtons);
					var rowPos = 0;
					var currentPercentageRow = POSTheme.CreateRowForGrid (this);
					_paymentSpecificButtonArea.AddView (currentPercentageRow);
					foreach (var percent in percentAmounts) {
						var percentButton = new PayPercentageButton (this, percent, POSTheme);
						POSTheme.SetButtonEnabled(percentButton, _vm.SetPaymentAmountAsPercentageOfCheck.CanExecute(percent));
						percentButton.Click += (senderPC, ePC) => {
							var perB = senderPC as PayPercentageButton;
							if (perB != null) {
								if(_vm.SetPaymentAmountAsPercentageOfCheck.CanExecute(perB.Percent))
								{
									_vm.SetPaymentAmountAsPercentageOfCheck.Execute(perB.Percent);
								}
							}
						};
						if (rowPos > 2) {
							currentPercentageRow = POSTheme.CreateRowForGrid (this);
							_paymentSpecificButtonArea.AddView (currentPercentageRow);
							rowPos = 0;
						}
						currentPercentageRow.AddView (percentButton);
						rowPos++;
					}
				} else {
					//this is external credit.  Display a form to let them enter tip amount, the process the external
					_commands.Visibility = ViewStates.Gone;

					var payCheck = POSTheme.CreateButton (this, "Pay Check", POSTheme.PayCheckColor);
					payCheck.SetMinimumWidth (POSTheme.PaymentsScreenRightColumnWidth);
					payCheck.SetMinHeight (POSTheme.PayCheckHeight);
					POSTheme.MarkButtonAsEnabled (payCheck);
					payCheck.Click += (senderP, eP) => {
						//get the tip, add others
					};
				}
			} catch(Exception ex){
				Logger.HandleException (ex);
			}
		}
			

		void CompButtonClicked (object sender, EventArgs e)
		{
			try{
				_vm.SelectedPaymentType = PaymentType.CompAmount;
				SelectPaymentType ();

				var selectedTab = Model.SelectedCheck;

				_paymentSpecificButtonArea.RemoveAllViews ();


				_amountEdit.Text = _vm.MaxCompAmount.ToString("C");

				var dollarAmounts = _vm.PossibleAmountsUnderAmount.Take(POSTheme.NumberOfCashButtons - 1).ToList();
				//var dollarAmounts = Model.GetPossibleAmountsUnderAmount(remainingAmt, POSTheme.NumberOfCashButtons - 1).ToList();
				dollarAmounts.Add(_vm.MaxCompAmount);

				int rowPos = 0;
				var currentDollarAmountsRow = POSTheme.CreateRowForGrid (this);
				_paymentSpecificButtonArea.AddView (currentDollarAmountsRow);
				foreach (var amt in dollarAmounts) {
					var compAmountButton = new PayAmtButton(this, new Money(amt), POSTheme);
					compAmountButton.Click += (senderPB, ePB) => {
						var payB = senderPB as PayAmtButton;
						if (payB != null) {
							//make a comp payment, and add it to payments
							_vm.MakeCompPayment.Execute(payB.Amt);
						}
					};

					if (rowPos > 2) {
						currentDollarAmountsRow = POSTheme.CreateRowForGrid (this);
						_paymentSpecificButtonArea.AddView (currentDollarAmountsRow);
						rowPos = 0;
					}
					currentDollarAmountsRow.AddView (compAmountButton);
					rowPos++;
				}

				var spacer = new Space(this);
				spacer.SetMinimumHeight (POSTheme.PaymentSpaceHeight);
				_paymentSpecificButtonArea.AddView(spacer);

				var percentAmounts = _vm.PossiblePaymentPercentages.Take(POSTheme.NumberOfCashButtons);
				rowPos = 0;
				var currentPercentageRow = POSTheme.CreateRowForGrid (this);
				_paymentSpecificButtonArea.AddView (currentPercentageRow);
				foreach (var percent in percentAmounts) {
					var percentButton = new PayPercentageButton (this, percent, POSTheme);
					percentButton.Click += (senderPC, ePC) => {
						var perB = senderPC as PayPercentageButton;
						if (perB != null) {
							throw new NotImplementedException();
						}
					};
					if (rowPos > 2) {
						currentPercentageRow = POSTheme.CreateRowForGrid (this);
						_paymentSpecificButtonArea.AddView (currentPercentageRow);
						rowPos = 0;
					}
					currentPercentageRow.AddView (percentButton);
					rowPos++;
				}
			} catch(Exception ex){
				Logger.HandleException (ex);
			}
		}
			
			
		void CashButtonClicked (object sender, EventArgs e)
		{
			try{
				_vm.SelectedPaymentType = PaymentType.Cash;
				SelectPaymentType ();

				_amountEdit.Text = _vm.AmountStillDue.ToString("C");


				_paymentSpecificButtonArea.RemoveAllViews ();
				//load all our payment amounts!
				var fullPayButtons = _vm.PossibleAmountsAboveAmount.Take(POSTheme.NumberOfCashButtons - 1);

				int rowPos = 0;
				var currentPayAboveRow = POSTheme.CreateRowForGrid (this);

				EventHandler buttClickHandler = (senderPB, ePB) => {
					var payB = senderPB as PayAmtButton;
					if(_vm.MakeCashPayment.CanExecute(payB.Amt)){
						_vm.MakeCashPayment.Execute(payB.Amt);
					}
				};

				_paymentSpecificButtonArea.AddView (currentPayAboveRow);

				//first add the EXACT button
				var exact = new PayExactButton(this, new Money(_vm.AmountStillDue), POSTheme);
				exact.Click += buttClickHandler;
				currentPayAboveRow.AddView(exact);
				rowPos++;

				foreach (var amt in fullPayButtons) {
					var cashB = new PayAmtButton(this, new Money(amt), POSTheme);
					cashB.Click += buttClickHandler;
					if (rowPos > 2) {
						currentPayAboveRow = POSTheme.CreateRowForGrid (this);
						_paymentSpecificButtonArea.AddView (currentPayAboveRow);
						rowPos = 0;
					}
					currentPayAboveRow.AddView (cashB);
					rowPos++;
				}

				var spacer = new Space(this);
				spacer.SetMinimumHeight (POSTheme.PaymentSpaceHeight);
				_paymentSpecificButtonArea.AddView(spacer);

				var subPayAmounts = _vm.PossibleAmountsUnderAmount.Take(POSTheme.NumberOfCashButtons);
				rowPos = 0;
				var currentPayBelowRow = POSTheme.CreateRowForGrid (this);
				_paymentSpecificButtonArea.AddView (currentPayBelowRow);
				foreach(var amt in subPayAmounts){
					var cashB = new PayAmtButton (this, new Money(amt), POSTheme);
					cashB.Click += buttClickHandler;
					if(rowPos > 2){
						currentPayBelowRow = POSTheme.CreateRowForGrid (this);
						_paymentSpecificButtonArea.AddView (currentPayBelowRow);
						rowPos = 0;
					}
					currentPayBelowRow.AddView (cashB);
					rowPos++;
				}
			} catch(Exception ex){
				Logger.HandleException (ex);
			}
		}

		void GratuityAndDiscountsClicked (object sender, EventArgs e)
		{
			_vm.SelectedPaymentType = null;
			SelectPaymentType ();
			POSTheme.MarkAsSelected (_addGratButton);
			_amountEdit.Enabled = false;

			_paymentSpecificButtonArea.RemoveAllViews ();

			//we'll want to load up both
			var gratuities = Model.GetPossibleGratuities ();

			var rowPos = 0;
			var currentRow = POSTheme.CreateRowForGrid (this);
			_paymentSpecificButtonArea.AddView (currentRow);
			foreach(var grat in gratuities){
				var gratButton = new DiscountButton (this, grat, POSTheme);
				gratButton.Click += (senderG, eG) => {
					var realButton = senderG as DiscountButton;
					if(realButton.Selected == false){
						Model.AddDiscountsToSelectedCheck (new []{ realButton.Discount });
						realButton.Select ();
					} else {
						Model.RemoveDiscountsFromSelectedCheck (new []{realButton.Discount});
						realButton.Deselect ();
					}
					PopulateOrderItems ();
					UpdateCommandsStatus(_vm);
				};
				if(Model.SelectedCheck.GetDiscounts().Contains (grat)){
					gratButton.Select ();
				} else {
					gratButton.Deselect ();
				}
				if(rowPos > 2){
					currentRow = POSTheme.CreateRowForGrid (this);
					_paymentSpecificButtonArea.AddView (currentRow);
					rowPos = 0;
				}
				currentRow.AddView (gratButton);
				rowPos++;
			}

			var discounts = Model.GetPossibleDiscounts ();
			currentRow = POSTheme.CreateRowForGrid (this);
			_paymentSpecificButtonArea.AddView (currentRow);
			foreach(var disc in discounts){
				var discButton = new DiscountButton (this, disc, POSTheme);
				discButton.Click += (senderD, eD) => {
					var realButton = senderD as DiscountButton;
					if(realButton.Selected == false){
						Model.AddDiscountsToSelectedCheck (new []{ realButton.Discount });
						realButton.Select ();
					} else {
						Model.RemoveDiscountsFromSelectedCheck (new []{realButton.Discount});
						realButton.Deselect ();
					}
					PopulateOrderItems ();
					UpdateCommandsStatus (_vm);
				};
				if(Model.SelectedCheck.GetDiscounts().Contains (disc)){
					discButton.Select ();
				} else {
					discButton.Deselect ();
				}
				if(rowPos > 2){
					currentRow = POSTheme.CreateRowForGrid (this);
					_paymentSpecificButtonArea.AddView (currentRow);
					rowPos = 0;
				}
				currentRow.AddView (discButton);
				rowPos++;
			}
		}


	}
}

