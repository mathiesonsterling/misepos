using System;
using Mise.Core.Services;

namespace Mise.RestaurantServer.Services
{
	public class SystemTraceLogger : ILogger
    {
        public void Log(string message, LogLevel level = LogLevel.Error)
        {
            var msg = message;
            switch (level)
            {
                case LogLevel.Info:
                    Info(msg);
                    break;
                case LogLevel.Debug:
                    Debug(msg);
                    break;
                case LogLevel.Error:
                    Error(msg);
                    break;
                case LogLevel.Warn:
                    Warn(msg);
                    break;
                case LogLevel.Fatal:
                    Fatal(msg);
                    break;
            }
        }

        public void HandleException(Exception ex, LogLevel level = LogLevel.Error)
        {
            var msg = ex.Message + "::" + ex.StackTrace;
            switch (level)
            {
                case LogLevel.Info:
                    Info(msg);
                    break;
                case LogLevel.Debug:
                    Debug(msg);
                    break;
                case LogLevel.Error:
                    Error(msg);
                    break;
                case LogLevel.Warn:
                    Warn(msg);
                    break;
                case LogLevel.Fatal:
                    Fatal(msg);
                    break;
            }
        }

		public void Debug(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        public void Info(string msg)
        {
            System.Diagnostics.Trace.TraceInformation(msg);
        }

        public void Error(string msg)
        {
            System.Diagnostics.Trace.TraceError(msg);
        }

        public void Warn(string msg)
        {
            System.Diagnostics.Trace.TraceWarning(msg);
        }

        public void Fatal(string msg)
        {
            System.Diagnostics.Trace.TraceError(msg);
        }

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