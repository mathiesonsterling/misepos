using System;

namespace Mise.Core.ValueItems
{
	public enum OAuthProviders{
		Facebook
	}
	public class OAuthToken : IEquatable<OAuthToken>
	{
		public OAuthProviders Provider{get;set;}
		public string Token{get;set;}

		#region IEquatable implementation

		public bool Equals (OAuthToken other)
		{
			if(other == null){
				return false;
			}

			return other.Provider == Provider && Token == other.Token;
		}

		#endregion
	}
}

