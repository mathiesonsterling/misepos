using System;
namespace Mise.Core.ValueItems
{
    /// <summary>
    /// What we can store of a credit card
    /// </summary>
    public class CreditCard : IEquatable<CreditCard>, ITextSearchable
    {
        public PersonName Name { get; set; }

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

            var res = Name.Equals(other.Name);
            return res;
		}
			

        public bool ContainsSearchString(string searchString)
        {
            return Name.ContainsSearchString(searchString);
        }
    }
}
