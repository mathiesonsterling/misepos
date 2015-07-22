using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems.Reports;

namespace Mise.Inventory.Reports
{
    public abstract class BaseReport
    {
        public abstract ReportResult RunReport();
    }
}
