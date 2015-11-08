using System;
using Mise.Core.Entities.Payments;

namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Represents a credit card number, CSV, and other information.  Cannot be stored ever!
	/// </summary>
	public class CreditCardNumber : IEquatable<CreditCardNumber>
	{
		public CreditCardNumber(){
			Number = string.Empty;	
		}

        public CreditCardNumber(string number, int csv, ZipCode billingZip, int month, int year){
			Number = number;
			CVC = csv;
			BillingZip = billingZip;
            ExpMonth = month;
            ExpYear = year;
		}

		public string Number{ get; set;}
		public int CVC{get;set;}
		public ZipCode BillingZip{ get; set;}
		public int ExpMonth { get; set; }
		public int ExpYear { get; set; }

		#region IEquatable implementation

		public bool Equals (CreditCardNumber other)
		{
			return Number == other.Number 
				&& CVC == other.CVC 
				&& ExpMonth == other.ExpMonth 
				&& ExpYear == other.ExpYear;
		}

		#endregion
	}
}

