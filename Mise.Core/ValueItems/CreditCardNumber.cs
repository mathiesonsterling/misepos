using System;
using Mise.Core.Entities.Payments;

namespace Mise.Core
{
	/// <summary>
	/// Represents a credit card number, CSV, and other information.  Cannot be stored ever!
	/// </summary>
	public class CreditCardNumber : IEquatable<CreditCardNumber>
	{
		public CreditCardNumber(){
			Number = string.Empty;	
		}

		public CreditCardNumber(string number, int csv, int billingZip){
			Number = number;
			CSV = csv;
			BillingZip = billingZip;
		}

		public string Number{ get; set;}
		public int CSV{get;set;}
		public int BillingZip{ get; set;}

		#region IEquatable implementation

		public bool Equals (CreditCardNumber other)
		{
			return Number == other.Number && CSV == other.CSV;
		}

		#endregion
	}
}

