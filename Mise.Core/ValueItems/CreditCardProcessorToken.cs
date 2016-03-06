using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    public enum CreditCardProcessors
    {
        FakeProcessor,
        Mercury,
        BluePay,
		Stripe,
        Apple
    }

    public class CreditCardProcessorToken : IEquatable<CreditCardProcessorToken>
    {
        public CreditCardProcessors Processor { get; set; }
        public string Token { get; set; }
        public bool Equals(CreditCardProcessorToken other)
        {
            return other != null
                   && Processor == other.Processor
                   && Token == other.Token;
        }
    }
}
