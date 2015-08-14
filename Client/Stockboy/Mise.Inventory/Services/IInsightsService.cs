using System;
using System.Collections.Generic;
using Mise.Core.Services.UtilityServices;

namespace Mise.Inventory.Services
{
    public interface IInsightsService : IErrorTrackingService
    {
        //Insights.Track("User Logged In", new Dictionary<string, string>{{"Email", email.Value}});
        void Track(string evName, Dictionary<string, string> values);
		void Track (string evName, string tagName, string value);


        IDisposable TrackTime(string name);
    }
}
