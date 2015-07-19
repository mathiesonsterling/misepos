using System;
using System.Text.RegularExpressions;
using Mise.Core.ValueItems;

namespace Mise.Core.ValueItems
{
	public class EmailAddress : IEquatable<EmailAddress>, ITextSearchable
	{
	    public EmailAddress()
	    {
	        Value = string.Empty;
	    }

	    public EmailAddress(string value)
	    {
	        Value = value;
	    }

		public string Value{ get; set;}

		public static bool IsValid(string email)
		{
			if (email == null) {
				return false;
			}
			string expresion;
			expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
			if (Regex.IsMatch (email, expresion)) {
				return Regex.Replace (email, expresion, string.Empty).Length == 0;
			}
			return false;
		}

        public override bool Equals(object obj)
        {
            var other = obj as EmailAddress;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

		public bool Equals(EmailAddress other){
			if(Value == null || other == null || other.Value == null){
				return false;
			}
			return Value.ToUpper().Equals (other.Value.ToUpper());
		}

		public override int GetHashCode(){
			return Value.ToUpper().GetHashCode ();
		}

	    public bool ContainsSearchString(string searchString)
	    {
			return string.IsNullOrEmpty(Value) == false && Value.ToUpper().Contains(searchString);
	    }

	    public static EmailAddress TestEmail { get { return new EmailAddress("test@test.com");} }
	}
}

