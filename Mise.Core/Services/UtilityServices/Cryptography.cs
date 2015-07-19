using System;
using System.Text;
using PCLCrypto;
namespace Mise.Core.Services.UtilityServices
{
	public static class Cryptography
	{
		public static string CalculateSha1Hash(string input)
		{
			// step 1, calculate MD5 hash from input
			var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1);
			var inputBytes = Encoding.UTF8.GetBytes(input);
			var hash = hasher.HashData(inputBytes);

			var sb = new StringBuilder();
			foreach (var t in hash)
			{
			    sb.Append(t.ToString("X2"));
			}
		    return sb.ToString();
		}

        /// <summary>
        /// Uses our key to encrypt for transmission
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
	    public static string EncryptString(string input)
	    {
	        throw new NotImplementedException();
	    }

	    public static string DecryptString(string input)
	    {
	        throw new NotImplementedException();
	    }
	}
}

