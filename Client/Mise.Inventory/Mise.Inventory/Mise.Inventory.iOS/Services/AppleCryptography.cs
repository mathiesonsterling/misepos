using System;
using System.IO;
using Mise.Core.Services.UtilityServices;
using System.Text;
using System.Security.Cryptography;
namespace Mise.Inventory.iOS.Services
{
    public class AppleCryptography : ICryptography
    {
        public string CalculateSha1Hash (string input)
        {
            var sha1 = SHA1.Create ();
            var inputBytes = Encoding.UTF8.GetBytes (input);
            var hash = sha1.ComputeHash (inputBytes);

            var sb = new StringBuilder ();
            foreach (var t in hash) {
                sb.Append (t.ToString ("X2"));
            }
            return sb.ToString ();
        }

        private Stream GenerateStreamFromString (string s)
        {
            var stream = new MemoryStream ();
            var writer = new StreamWriter (stream);
            writer.Write (s);
            writer.Flush ();
            stream.Position = 0;
            return stream;
        }
    }
}

