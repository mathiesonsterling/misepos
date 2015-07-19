using System;

namespace Mise.Core.ValueItems
{
    public class ReferralCode : IEquatable<ReferralCode>, ITextSearchable
    {
        public ReferralCode() { }

        public ReferralCode(string code)
        {
            Code = code;
        }

        private string _code;

        public string Code
        {
            get { return _code; }
            set { _code = value.ToUpper(); }
        }

        public bool Equals(ReferralCode other)
        {
            return other != null && String.Equals(Code.ToUpper(), other.Code.ToUpper(), StringComparison.CurrentCultureIgnoreCase);
        }

        public static ReferralCode TestReferralCode { get { return new ReferralCode {Code = "TESTING"}; } }
        public bool ContainsSearchString(string searchString)
        {
            return string.IsNullOrEmpty(Code) == false && Code.Contains(searchString);
        }
    }
}
