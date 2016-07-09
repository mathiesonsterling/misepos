using System;
using Mise.Core.Services.UtilityServices;
using System.Linq;
namespace Mise.Core.Common.UnitTests.Tools
{
    public class TestCrypto : ICryptography
    {

        public string CalculateSha1Hash (string input)
        {
            return new string(("$aa" + input).Reverse ().ToArray ());
        }
    }
}

