using System;

namespace Mise.Core.ValueItems
{
    public class CreditCard : IEquatable<CreditCard>, ITextSearchable
    {
        public CreditCard() { }

		public CreditCard(CreditCardProcessorToken token, PersonName name, int expMonth, int expYear, int? verificationCode, ZipCode zip)
        {
			ProcessorToken = token;
            Name = name;
            ExpMonth = expMonth;

			if(expYear < 100){
				expYear = expYear + 2000;
			}
            ExpYear = expYear;
			BillingZip = zip;
        }

        public PersonName Name { get; set; }

        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }

		public ZipCode BillingZip{ get; set;}

        /// <summary>
        /// The token we've gotten from our credit card processor.  Once we have this, we discard everything else sensitive
        /// </summary>
        public CreditCardProcessorToken ProcessorToken { get; set; }

			
        public bool Equals (CreditCard other)
		{
            if (other == null)
            {
                return false;
            }
			if(ProcessorToken != null && ProcessorToken.Equals (other.ProcessorToken)){
				return true;
			}

            var res = Name.Equals(other.Name) && ExpMonth == other.ExpMonth &&
                      ExpYear == other.ExpYear;
            return res;
		}
			

        public bool ContainsSearchString(string searchString)
        {
            return 
            ExpMonth.ToString().Contains(searchString)
            ||
            (ExpYear.ToString().Contains(searchString))
            ||
            Name.ContainsSearchString(searchString);
        }

		public static bool CardNumberIsValid(string num){
			return string.IsNullOrEmpty (num) == false
			&& num.Length > 12 && num.Length < 17;
		}
    }
}
