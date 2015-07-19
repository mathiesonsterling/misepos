using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Xamarin.Forms;

using Mise.Core.Client.ApplicationModel;
using Mise.Core.Services;
using Mise.Core.Entities.Payments;
using Mise.Core.ValueItems;

namespace Mise.POSTerminal.ViewModels
{
	public class PaymentsViewModel : BaseViewModel
	{
		#region implemented abstract members of BaseViewModel

		public override void CreditCardSwiped(CreditCard card)
		{
			if (InTipMode) {
				//get the payment with this card on it
				var thisPayment = Model.SelectedCheck.GetPayments()
					.OfType<ICreditCardPayment>()
					.FirstOrDefault(p => p.Card == card);
				if (thisPayment != null) {
					//select it!
					SelectedPayment = thisPayment;
					if (OnLoadPaymentForTipAmount != null) {
						OnLoadPaymentForTipAmount(this);
					}
				}
			} else {
				//depending on where we are, do stuff!
				CreditCardNumber = card.CardNumber;
				CreditCardMonth = card.ExpMonth;
				CreditCardYear = card.ExpYear;

				SelectedPaymentType = PaymentType.InternalCreditCard;
				if (OnUpdateSelectedPaymentType != null) {
					OnUpdateSelectedPaymentType(this);
				}
			}
		}

		#endregion

		#region Updatable Properties

		string _creditCardNumber;

		public string CreditCardNumber {
			get { return _creditCardNumber; }
			set {
				_creditCardNumber = value;
				OnPropertyChanged("CreditCardNumber");
			}
		}

		public int? CreditCardMonth {
			get { return _creditCardMonth; }
			set {
				_creditCardMonth = value;
				OnPropertyChanged("CreditCardMonth");
			}
		}

		public int? CreditCardYear {
			get { return _creditCardYear; }
			set {
				_creditCardYear = value;
				OnPropertyChanged("CreditCardYear");
			}
		}

		decimal _creditCardAmount;

		public decimal CreditCardAmount {
			get { return _creditCardAmount; }
			set {
				_creditCardAmount = value;
				OnPropertyChanged("CreditCardAmount");
			}
		}

		bool _canPayCheck;

		public bool CanPayCheck {
			get { return _canPayCheck; }
			set {
				_canPayCheck = value;
				OnPropertyChanged("CanPayCheck");
			}
		}

		public bool CanAddCharge {
			get {
				throw new NotImplementedException("");
			}
		}


		decimal? _tipAmount;

		public decimal? TipAmount {
			get { return _tipAmount; }
			set {
				_tipAmount = value;
				OnPropertyChanged("TipAmount");
			}
		}

		decimal _checkTotal;

		public decimal CheckTotal { 
			get { return _checkTotal; } 
			private set { 
				_checkTotal = value; 
				OnPropertyChanged("CheckTotal"); 
			} 
		}

		decimal _amountStillDue;

		public decimal AmountStillDue { 
			get { 
				return _amountStillDue;
				//return Model.GetRemainingAmountToBePaidOnCheck (); 
			} 
			private set {
				_amountStillDue = value;
				OnPropertyChanged("AmountStillDue");
			}
		}

		decimal _amountToPay;

		public decimal AmountToPay {
			get {
				return _amountToPay;
			}
			private set {
				_amountToPay = value;
				OnPropertyChanged("AmountToPay");
			}
		}

		IProcessingPayment _selectedPayment;
		int? _creditCardMonth;
		int? _creditCardYear;

		public IProcessingPayment SelectedPayment {
			get { return _selectedPayment; }
			set {
				_selectedPayment = value;
				OnPropertyChanged("SelectedPayment");
			}
		}

		private PaymentType? _selectedPaymentType;

		public PaymentType? SelectedPaymentType {
			get{ return _selectedPaymentType; }
			set {
				_selectedPaymentType = value;
				OnPropertyChanged("SelectedPaymentType");
			}
		}

		#endregion

		#region Properties

		public bool InTipMode {
			get{ return Model.SelectedCheck.PaymentStatus == CheckPaymentStatus.PaymentApprovedWithoutTip; }
		}

		public bool UseIntegratedCredit {
			get{ return Model.UseIntegratedCredit; }
		}

		public bool CanCompAmounts {
			get {
				return Model.SelectedEmployee != null && Model.SelectedEmployee.CanCompAmount;
			}
		}

		public bool CanTakeCash {
			get{ return Model.HasCashDrawer; }
		}

		public decimal MaxCompAmount {
			get {
				var employeeBudget = Model.SelectedEmployee.CompBudget ?? Money.None;

				return AmountStillDue > employeeBudget.Dollars ? employeeBudget.Dollars : AmountStillDue;
			}
		}

		#endregion

		public IEnumerable<IPayment> Payments { get { return Model.SelectedCheck.GetPayments(); } }

		public IEnumerable<ICreditCardPayment> WaitingForTipPayments { get { return Model.WaitingForTipPayments.OfType<ICreditCardPayment>(); } }

		public IEnumerable<decimal> PossiblePaymentPercentages {
			get {
				//var sources = new List<decimal>{ .1M, .5M, 1M, .25M, .20M, .33M, .3M, .66M,  .75M, .8M };

				var sources = new List<decimal>{ .5M, .33M, .25M, .10M, .20M, .66M, .75M };
				//get the amounts 
				return sources;
			}
		}

		public IEnumerable<decimal> PossibleAmountsUnderAmount {
			get {
				const int MAX_NUM = 10;
				var amts = new List<Decimal>();
				if (AmountStillDue > 1.0M) {
					var lastAmount = 0.0M;

					bool has5 = false;
					bool has10 = false;
					while (lastAmount < AmountStillDue && amts.Count < MAX_NUM) {
						lastAmount = lastAmount + 1.0M;
						if (IsBestNextBillAmount(lastAmount, ref has5, ref has10)) {
							if (amts.Contains(lastAmount) == false && lastAmount > 0) {
								amts.Add(lastAmount);
								yield return lastAmount;
							}
						}
					}
				}
			}
		}

		public IEnumerable<decimal> PossibleAmountsAboveAmount {
			get {
				var nearestDol = (decimal)(Math.Ceiling((double)AmountStillDue));
				var res = new List<decimal> { nearestDol };

				const int MAX_NUM = 10;
				//go up till we've got items
				decimal curr = nearestDol;
				bool found5 = false;
				bool found10 = false;

				bool firedFirst = false;
				while (res.Count < MAX_NUM) {
					if (firedFirst == false) {
						firedFirst = true;
						yield return nearestDol;
					}
					curr = curr + 1.0M;
					if (IsBestNextBillAmount(curr, ref found5, ref found10)) {
						if (res.Contains(curr) == false && curr > 0) {
							res.Add(curr);
							yield return curr;
						}
					}
				}
			}
		}

		/// <summary>
		/// Helper function, tests a bill to see if it should be displayed
		/// </summary>
		/// <returns><c>true</c> if is best next bill amount the specified curr foundNearest5 foundNearest10; otherwise, <c>false</c>.</returns>
		/// <param name="curr">Curr.</param>
		/// <param name="foundNearest5">Found nearest5.</param>
		/// <param name="foundNearest10">Found nearest10.</param>
		static bool IsBestNextBillAmount(decimal curr, ref bool foundNearest5, ref bool foundNearest10)
		{
			if (foundNearest5 == false) {
				if ((curr % 5) == 0) {
					foundNearest5 = true;
					return true;
				}
			}

			if (foundNearest10 == false) {
				if ((curr % 10) == 0) {
					foundNearest10 = true;
					foundNearest5 = true;
					return true;
				}
			}

			if ((curr % 20) == 0) {
				foundNearest5 = true;
				foundNearest10 = true;
				return true;
			}
			return (curr % 50) == 0;
		}

		#region Events

		/// <summary>
		/// Fired when we need to update the payments that are listed
		/// </summary>
		public event ViewModelEventHandler OnUpdatePayments;

		public event ViewModelEventHandler OnLoadPaymentForTipAmount;

		public void LoadPaymentForTipAmount()
		{
			if (SelectedPayment == null) {
				SelectedPayment = WaitingForTipPayments.FirstOrDefault();
			}
			if (SelectedPayment == null) {
				throw new ArgumentException("No payments are waiting for tips here!");
			}
			if (OnLoadPaymentForTipAmount != null) {
				OnLoadPaymentForTipAmount(this);
			}
		}

		/// <summary>
		/// Fired when we need to update the payment methods - some are no longer available, we select a new one, etc
		/// </summary>
		public event ViewModelEventHandler OnUpdateSelectedPaymentType;
		public event ViewModelEventHandler OnCommandsUpdateStatus;
		public event ViewModelEventHandler UpdateAmountInPayment;

		#endregion

		/// <summary>
		/// Adds a charge to the ticket
		/// </summary>
		/// <value>The charge card.</value>
		public ICommand AddCreditCardCharge { get; private set; }

		/// <summary>
		/// Adds a tip to the given charge
		/// </summary>
		/// <value>The add tip.</value>
		public ICommand AddTip { get; private set; }

		void DoAddTip()
		{
			if (TipAmount.HasValue) {
				var tip = new Money(TipAmount.Value);
				TipAmount = null;
				//tell view model we've done this, and move on!
				var res = Model.AddTipToProcessingPayment(SelectedPayment, tip);
				if (res) {
					//do we have more payments?
					if (WaitingForTipPayments.Any()) {
						//move to the next, and update our display!
						SelectedPayment = Model.WaitingForTipPayments.First();
						if (OnLoadPaymentForTipAmount != null) {
							OnLoadPaymentForTipAmount(this);
						}
					} else {
						//we've updated all payments waiting for a tip, so we can move out of here
//						MoveToView(TerminalViewTypes.ViewChecks);
					}
				}
				//error
			} else {//error, no tip amount set
				throw new ArgumentException("No tip amount was set!");
			}
		}

		public ICommand VoidAuthorization { get; private set; }

		void DoVoidAuthorization()
		{
			var res = Model.VoidAuthorizedProcessingPayment(SelectedPayment);
			if (res) {
				//stay or go?
//				MoveToView(Model.CurrentTerminalViewTypeToDisplay);
			} else {
				if (OnUpdatePayments != null) {
					OnUpdatePayments(this);
				}

				//TODO where do we go now?  Update our selected payment at least!
			}
		}

		public ICommand Reopen { get; private set; }

		void DoReopen()
		{
			CheckTotal = Model.GetTotalWithSalesTaxForSelectedCheck().Dollars;
			Model.ReopenSelectedCheck();
//			MoveToView(TerminalViewTypes.OrderOnCheck);
		}


		public ICommand Cancel { get; private set; }

		public ICommand Save { get; private set; }

		public ICommand Pay { get; private set; }

		public ICommand MakeCashPayment{ get; private set; }

		void DoMakeCashPayment(Money amt)
		{
			var res = Model.PayCheckWithCash(amt);

			if (res) {
				AmountStillDue = Model.GetRemainingAmountToBePaidOnCheck().Dollars;
				//if we paid the entire bill, AND have no other payments, we should jump out at this point to change
				if (Model.SelectedCheck.GetPayments().Count() == 1) {
					if (Pay.CanExecute(null)) {
						Pay.Execute(null);
						return;
					}
				}
				LoadAfterPayment();
			} else {
				Logger.Log("Error attempting to make cash payment");
			}
		}

		public ICommand MakeInternalCreditPayment{ get; private set; }

		void DoMakeInternalCreditPayment(Money amt)
		{
			if (CreditCardYear.HasValue == false && CreditCardMonth.HasValue == false) {
				throw new ArgumentException("Credit Card or Year not set!");
			}
			var card = new CreditCard {
				CardNumber = CreditCardNumber,
				// ReSharper disable PossibleInvalidOperationException
				ExpYear = CreditCardYear.Value,
				ExpMonth = CreditCardMonth.Value
				// ReSharper restore PossibleInvalidOperationException
			};
			var res = Model.PayCheckWithCreditCard(card, amt);
			if (res) {
				AmountStillDue = Model.GetRemainingAmountToBePaidOnCheck().Dollars;
				CreditCardNumber = string.Empty;
				CreditCardYear = null;
				CreditCardMonth = null;
				LoadAfterPayment();
			} else {
				var msg = string.Format("Error trying to make credit payment on card {0}", card.CardNumber);
				Logger.Log(msg);
			}
		}

		public ICommand MakeCompPayment{ get; private set; }

		void DoMakeCompPayment(Money amt)
		{
			var res = Model.PayCheckWithAmountComp(amt);
			if (res) {
				AmountStillDue = Model.GetRemainingAmountToBePaidOnCheck().Dollars;
				LoadAfterPayment();
			} else {
				Logger.Log("Error attempting to add comp payment");
			}
		}

		public ICommand SetPaymentAmountAsPercentageOfCheck{ get; private set; }

		/// <summary>
		/// Method to fire all events that need to happen after a payment is made
		/// </summary>
		void LoadAfterPayment()
		{
			SelectedPaymentType = null;

			if (OnCommandsUpdateStatus != null) {
				OnCommandsUpdateStatus(this);
			}

			//reload the payments
			if (OnUpdatePayments != null) {
				OnUpdatePayments(this);
			}

			if (OnUpdateSelectedPaymentType != null) {
				OnUpdateSelectedPaymentType(this);
			}
		}

		public PaymentsViewModel(ILogger logger, ITerminalApplicationModel model)
			: base(logger, model)
		{
			AddTip = new Command(
				DoAddTip,
				() => TipAmount.HasValue && SelectedPayment != null
			);
			Reopen = new Command(
				DoReopen,
				() => Model.Dirty == false
			);
			VoidAuthorization = new Command(
				DoVoidAuthorization, () => SelectedPayment != null
			);
			Cancel = new Command(() => {
				Model.CancelPayments();
//				MoveToView(Model.CurrentTerminalViewTypeToDisplay);
			}
			);

			Save = new Command(delegate {
				var res = Model.SavePaymentsClicked();
				if (res) {
//					MoveToView(Model.CurrentTerminalViewTypeToDisplay);
				}
			}, () => Model.Dirty);

			Pay = new Command(
				delegate {
					//we can pay and move on
					var check = Model.MarkSelectedCheckAsPaid();
					CheckTotal = Model.GetTotalWithSalesTaxForSelectedCheck().Dollars;
					if (check != null) {
//						MoveToView(Model.CurrentTerminalViewTypeToDisplay);
					}
				},
				() => AmountStillDue <= 0
			);

			MakeCashPayment = new Command<Money>(DoMakeCashPayment, amt => amt.HasValue && Model.SelectedCheck != null);

			MakeInternalCreditPayment = new Command<Money>(DoMakeInternalCreditPayment,
				amt => 
				amt.HasValue
				&& string.IsNullOrEmpty(CreditCardNumber) == false
				&& CreditCardNumber.Length >= 15
				&& CreditCardMonth.HasValue
				&& CreditCardYear.HasValue
				&& (CreditCardYear.Value > DateTime.UtcNow.Year || (CreditCardYear.Value == DateTime.UtcNow.Year && CreditCardMonth.Value >= DateTime.UtcNow.Month))
			);

		
			MakeCompPayment = new Command<Money>(
				DoMakeCompPayment, 
				amt => amt.HasValue && AmountStillDue >= amt.Dollars
			);

			SetPaymentAmountAsPercentageOfCheck = new Command<decimal>(
				percentage => AmountToPay = GetPercentageOfCheckTotal(percentage),
				percentage => GetPercentageOfCheckTotal(percentage) <= (AmountStillDue + .01M)
			);

			var remAmt = Model.GetRemainingAmountToBePaidOnCheck();
			AmountStillDue = remAmt != null ? remAmt.Dollars : 0.0M;
			CheckTotal = Model.GetTotalWithSalesTaxForSelectedCheck().Dollars;
		}

		/// <summary>
		/// Given a percentage we want to display of the check total, display it
		/// </summary>
		/// <returns>The percentage of check total.</returns>
		/// <param name="percetage">Percetage.</param>
		public decimal GetPercentageOfCheckTotal(decimal percetage)
		{
			//get the percentage, and round up if we're over a cent at all
			var amt = CheckTotal * percetage;
			if ((amt % .01M) > 0) {
				var asInt = (int)(amt * 100);
				asInt += 1;
				amt = asInt * 1 / 100.0M;
			}
			return amt;
		}
	}
}

