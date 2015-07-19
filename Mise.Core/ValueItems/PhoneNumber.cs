using System;

namespace Mise.Core.ValueItems
{
	public class PhoneNumber : IEquatable<PhoneNumber>, ITextSearchable
	{
		public string AreaCode{get;set;}
		public string Number{get;set;}
	    public bool Equals(PhoneNumber other)
	    {
	        var thisAreaCode = CleanAreaCode(AreaCode);
	        var otherAreaCode = CleanAreaCode(other.AreaCode);

	        var thisNumer = CleanNumber(Number);
	        var otherNumber = CleanNumber(other.Number);

	        return thisAreaCode.Equals(otherAreaCode) &&
	               thisNumer.Equals(otherNumber);

	    }

		public PhoneNumber(){
			
		}

		public PhoneNumber(string areaCode, string number){
			AreaCode = CleanAreaCode (areaCode);
			Number = CleanNumber (number);
		}

	    private static string CleanAreaCode(string areaCode)
	    {
			if(areaCode == null){
				return string.Empty;
			}
	        return areaCode.Replace("(", "").Replace(")", "").Replace(" ", "");
	    }

	    private static string CleanNumber(string number)
	    {
			if(number == null){
				return string.Empty;
			}
	        return number.Replace("-", "").Replace(" ", "");
	    }

	    public bool ContainsSearchString(string searchString)
	    {
	        var cleanArea = CleanAreaCode(AreaCode);
	        var cleanNum = CleanNumber(Number);
	        return cleanArea.Contains(searchString) || cleanNum.Contains(searchString);
	    }

	    public static PhoneNumber TestPhoneNumber
	    {
	        get
	        {
	            return new PhoneNumber
	            {
	                AreaCode = "(111)",
                    Number = "222 - 3332"
	            };
	        }
	    }
	}
}

