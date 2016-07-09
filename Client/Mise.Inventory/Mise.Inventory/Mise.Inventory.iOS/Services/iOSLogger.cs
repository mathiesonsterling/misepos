﻿using System;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Xamarin;
namespace Mise.Inventory.iOS.Services
{
	public class IOSLogger : ILogger
	{
		#region ILogger implementation
		private const string TAG = "misePOS";
		public void Log (string message, LogLevel level = LogLevel.Error)
		{
			Console.WriteLine (level + ":" + message);
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
                    try{
					    Insights.Report (ex);
                    } catch(Exception e){
                    }
				}
                /*
				try{
					_errorTrack.ReportException (ex, level);
				} catch(Exception){
					Log (ex.Message, LogLevel.Fatal);
				}*/
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
