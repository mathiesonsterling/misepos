using System;

using Xamarin;
using Mise.Core.Services;

using AndroidOS = Android.Util;
namespace Mise.AndroidCommon.Services
{
	public class AndroidClientLogger : ILogger
	{
		public AndroidClientLogger ()
		{
		}

		#region ILogger implementation
		private const string TAG = "misePOS";
		public void Log (string message, LogLevel level = LogLevel.Error)
		{
			switch (level) {
				case LogLevel.Debug:
					AndroidOS.Log.Debug (TAG, message);
					break;
				case LogLevel.Error:
					AndroidOS.Log.Error (TAG, message);
					break;
				case LogLevel.Fatal:
					AndroidOS.Log.Error (TAG, message);
					break;
				case LogLevel.Info:
					AndroidOS.Log.Info (TAG, message);
					break;
			}
		}

		public void Debug (string message)
		{
			Log (message, LogLevel.Debug);
		}

		public void Info (string message)
		{
			Log (message, LogLevel.Info);
		}

		public void Warn (string message)
		{
			Log (message, LogLevel.Warn);
		}

		public void Error (string message)
		{
			Log (message, LogLevel.Error);
		}

		public void Fatal (string message)
		{
			Log (message, LogLevel.Fatal);
		}

		public void HandleException (Exception ex, LogLevel level = LogLevel.Error)
		{
			if (ex != null) {
				if (level == LogLevel.Error || level == LogLevel.Fatal) {
					Insights.Report (ex);
				}
				Log (ex.Message + "::" + ex.StackTrace, level);
			} else {
				Log ("Null exception received", level);
			}
		}

		#endregion

		public void HandleFaultedTask (System.Threading.Tasks.Task t, LogLevel level = LogLevel.Error)
		{
			if (t.Exception != null) {
				foreach (var ex in t.Exception.Flatten().InnerExceptions) {
					HandleException (ex, level);
				}
			}
		}
	}
}

