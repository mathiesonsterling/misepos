using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mise.Core.Services;

using Xamarin;
namespace Mise.Inventory.Services.Implementation
{
	public class XamarinInsightsService : IInsightsService
    {
        public void Identify(string tag, string item, string value)
        {
            Xamarin.Insights.Identify(tag, item, value);
        }

        public void Track(string evName, Dictionary<string, string> values)
        {
            Insights.Track(evName, values);
        }

		public void Track (string evName, string tagName, string value)
		{
			var values = new Dictionary<string, string>{ { tagName, value } };
			Track (evName, values);
		}

        public IDisposable TrackTime(string name)
        {
            return Insights.TrackTime(name);
        }

		public void ReportException (Exception e, LogLevel level)
		{
			var severity = Insights.Severity.Warning;
			switch (level) {
			case LogLevel.Info:
			case LogLevel.Debug:
			case LogLevel.Warn:
				severity = Insights.Severity.Warning;
				break;
			case LogLevel.Error:
				severity = Insights.Severity.Error;
				break;
			case LogLevel.Fatal:
				severity = Insights.Severity.Critical;
				break;
			}
			Insights.Report (e, severity);
		}
    }
}
