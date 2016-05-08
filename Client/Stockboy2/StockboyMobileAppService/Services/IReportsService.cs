using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems.Reports;

namespace Mise.Inventory.Services
{
    public interface IReportsService
    {
        Task SetCurrentReportRequest(ReportRequest request);

        Task<ReportRequest> GetCurrentReportRequest();

        Task<ReportResult> RunCurrentReport();
    }
}
