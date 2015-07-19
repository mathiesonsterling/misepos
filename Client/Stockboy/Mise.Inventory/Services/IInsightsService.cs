using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin;
using XLabs.Platform.Services.Media;

using Mise.Core.Services;
namespace Mise.Inventory.Services
{
    public interface IInsightsService
    {
        void Identify(string tag, string item, string value);
        //Insights.Track("User Logged In", new Dictionary<string, string>{{"Email", email.Value}});
        void Track(string evName, Dictionary<string, string> values);
		void Track (string evName, string tagName, string value);

		void ReportException (Exception e, LogLevel level);

        IDisposable TrackTime(string name);
    }
}
