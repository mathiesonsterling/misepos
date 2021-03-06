using System.Threading.Tasks;
using Mise.Core.Entities.Inventory;
using Mise.Core.ValueItems.Reports;

namespace Mise.Core.Common.Entities.Reports
{
    public abstract class BaseReport
    {
        public abstract ReportTypes ReportType { get; }

        protected abstract ReportResult CreateReport();

        public Task<ReportResult> RunReportAsync()
        {
            return Task.Run(() => CreateReport());
        }

        /// <summary>
        /// Give a list item, get a string key that is unique to it
        /// </summary>
        /// <param name="li"></param>
        /// <returns></returns>
        public static string GetListItemKey(IBaseBeverageLineItem li)
        {
            var key = li.DisplayName + "|" + li.MiseName + "|" + li.Container.DisplayName;
            return key;
        }
    }
}
