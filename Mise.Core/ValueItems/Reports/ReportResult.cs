using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Reports
{
    public class ReportResult
    {
        public ReportResult(ReportTypes type, string title, IEnumerable<ReportResultLineItem> lineItems, decimal checkSum)
        {
            Type = type;
            Title = title;
            LineItems = lineItems;
            Checksum = checkSum;
        }

        public ReportTypes Type { get; private set; }

        public string Title { get; private set; }
        public IEnumerable<ReportResultLineItem> LineItems { get; private set; }

        /// <summary>
        /// Some value we can use to tell if we get the same results running the item again
        /// </summary>
        public decimal Checksum { get; private set; }
    }
}
