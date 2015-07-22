using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Reports
{
    /// <summary>
    /// Single line of a report
    /// </summary>
    public class ReportResultLineItem : IEquatable<ReportResultLineItem>
    {
        public ReportResultLineItem(string mainText, string detailText, decimal? quantity, bool isErrored)
        {
            MainText = mainText;
            DetailText = detailText;
            Quantity = quantity;
            IsErrored = isErrored;
        }

        public string MainText { get; private set; }
        public string DetailText { get; private set; }
        /// <summary>
        /// If true, this item has an error
        /// </summary>
        public bool IsErrored { get; set; }

        public decimal? Quantity { get; set; }

        public bool Equals(ReportResultLineItem other)
        {
            if (other == null)
            {
                return false;
            }

            return MainText == other.MainText
                   && DetailText == other.DetailText
                   && IsErrored == other.IsErrored
                   && Quantity == other.Quantity;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ReportResultLineItem;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MainText != null ? MainText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DetailText != null ? DetailText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsErrored.GetHashCode();
                hashCode = (hashCode * 397) ^ Quantity.GetHashCode();
                return hashCode;
            }
        }
    }
}
