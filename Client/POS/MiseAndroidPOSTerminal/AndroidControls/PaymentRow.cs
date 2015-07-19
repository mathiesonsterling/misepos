using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Mise.Core.ValueItems;
using MiseAndroidPOSTerminal.Themes;
using Mise.Core.Entities.Payments;

namespace MiseAndroidPOSTerminal.AndroidControls
{
	sealed class PaymentRow : LinearLayout{
		public IPayment Payment{get;private set;}
		public PaymentRow(Context context, IMiseAndroidTheme theme, IPayment payment) : base(context){
			Orientation = Orientation.Horizontal;
			Payment = payment;

			//add our info
			string payLabel;
			switch(Payment.PaymentType){
			case PaymentType.CompAmount:
				payLabel = "Comp";
				break;
			case PaymentType.InternalCreditCard:
				payLabel = "Credit Card";
				break;
			default:
				payLabel = Payment.PaymentType.ToString ();
				break;
			}
			var paymentTypeLabel = new TextView (context){ Text = payLabel };
			paymentTypeLabel.SetTextSize (ComplexUnitType.Pt, theme.PaymentTextSize);
			paymentTypeLabel.SetMinimumWidth (theme.PaymentTypeWidth);
			var payColor = theme.GetPaymentTypeColor (payment.PaymentType);
			paymentTypeLabel.SetTextColor (payColor);
			AddView (paymentTypeLabel);

			var payAmt = new TextView(context){ 
				Text = Payment.DisplayAmount.ToFormattedString ()
			};
			payAmt.SetTextColor (theme.DefaultTextColor);
			payAmt.SetTextSize (ComplexUnitType.Pt, theme.PaymentTextSize);
			payAmt.SetMinimumWidth (theme.PaymentAmountWidth);
			payAmt.Gravity = GravityFlags.Right;
			AddView (payAmt);

			//if we're a credit card, we add a third column for the status
			var statusField = new TextView (context);
			statusField.SetTextColor (theme.DefaultTextColor);
			statusField.SetTextSize(ComplexUnitType.Pt, theme.PaymentTextSize);
			statusField.Gravity = GravityFlags.Right;
			statusField.SetMinimumWidth (theme.PaymentAmountWidth);
			var processingPayment = Payment as IProcessingPayment;
			if(processingPayment != null){
				string status = string.Empty;
				switch(processingPayment.PaymentProcessingStatus){
				case PaymentProcessingStatus.Created:
					status = "Started";
					break;
				case PaymentProcessingStatus.BaseAuthorized:
					status = "Authorized";
					break;
				case PaymentProcessingStatus.Complete:
					status = "Complete";
					break;
				case PaymentProcessingStatus.BaseRejected:
				case PaymentProcessingStatus.FullAmountRejected:
					status = "Rejected";
					statusField.SetTextColor (theme.CancelButtonColor);
					break;
				case PaymentProcessingStatus.SentForBaseAuthorization:
				case PaymentProcessingStatus.SentForFullAuthorization:
					status = "Processing";
					break;
				}
				statusField.Text = status;
				AddView (statusField);
			} else {
				var cashPay = Payment as CashPayment;
				if(cashPay != null){
					var amtTendField = new TextView(context);
					amtTendField.SetTextColor (theme.CashButtonColor);
					amtTendField.SetTextSize(ComplexUnitType.Pt, theme.PaymentTextSize);
					amtTendField.Gravity = GravityFlags.Right;
					amtTendField.SetMinimumWidth (theme.PaymentAmountWidth);
					amtTendField.Text = cashPay.AmountTendered.ToFormattedString ();
					AddView(amtTendField);
				}
			}
		}
	}
}

