using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Reports
{
    public class ReportResult
    {
        public ReportResult(ReportTypes type, string title, IEnumerable<ReportResultLineItem> lineItems)
        {
            Type = type;
            Title = title;
            LineItems = lineItems;
        }

        public ReportTypes Type { get; private set; }

        public string Title { get; private set; }
        public IEnumerable<ReportResultLineItem> LineItems { get; private set; } 
    }
}
