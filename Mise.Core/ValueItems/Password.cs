using System;
using System.Text;
using System.Linq;
using Mise.Core.Services.UtilityServices;


namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Represents a password, but is hashed for protection over the wire
	/// </summary>
	public class Password : IEquatable<Password>
	{
	    public Password()
	    {
	        
	    }

	    public Password(string password)
	    {
	        SetValue(password);
	    }

		/// <summary>
		/// SHA-1 value of the password.  ONLY this can be sent across the wire
		/// </summary>
		/// <value><c></value>
		public string HashValue{get;set;}

		/// <summary>
		/// Sets the value given an unencrypted password string
		/// </summary>
		/// <param name="rawPassword">Raw password.</param>
		public void SetValue(string rawPassword){
		    if (string.IsNullOrEmpty(rawPassword))
		    {
		        throw new ArgumentException("Cannot set a password to an empty value!");
		    }
			var hash = Cryptography.CalculateSha1Hash (rawPassword);

			HashValue = hash;
		}
			

		#region IEquatable implementation
		public bool Equals (Password other)
		{
			return HashValue == other.HashValue;
		}
		#endregion

		public static bool IsValid(string password){
			return password != null && password.Length > 2;
		}

	    public static Password TestPassword 
	    {
            get { return new Password("test");}
	    }
	}
}

