using System;
using System.Collections.Generic;
using Akavache;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Xamarin;
namespace Mise.Inventory.Services.Implementation
{
	public class XamarinInsightsService : IInsightsService
    {
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

	    public void Identify(Guid? userID, EmailAddress email, PersonName name, string deviceID, bool isAnonymous)
	    {
            var values = new Dictionary<string, string>();
	        var userIDVal = string.Empty;
	        if (userID.HasValue)
	        {
	            userIDVal = userID.ToString();
	            values.Add("UserID", userIDVal);
	        }

	        if (email != null)
	        {
	            if (userID.HasValue == false)
	            {
	                userIDVal = email.Value;
	            }
                values.Add("Email", email.Value);
	        }

	        if(name != null)
	        {
	            values.Add("Name", name.ToSingleString());
	        }

	        if (string.IsNullOrEmpty(deviceID) == false)
	        {
	            values.Add("DeviceID", deviceID);
	        }

	        if (isAnonymous)
	        {
	            values.Add("Anonymous", "True");
	        }
	        Insights.Identify(userIDVal, values);
	    }
    }
}
