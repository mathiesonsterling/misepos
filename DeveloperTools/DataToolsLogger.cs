using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;

namespace DeveloperTools
{
    public class DataToolsLogger : ILogger
    {
        public void Log(string message, LogLevel level = LogLevel.Error)
        {
            MessageBox.Show(message);
        }

        public void Debug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            Log(message, LogLevel.Info);
        }

        public void Warn(string message)
        {
            Log(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            Log(message);
        }

        public void Fatal(string message)
        {
            Log(message, LogLevel.Fatal);
        }

        public void HandleException(Exception ex, LogLevel level = LogLevel.Error)
        {
            Log("Exception . . . " + ex.Message);
        }

        public void HandleFaultedTask(Task t, LogLevel level = LogLevel.Error)
        {
            if (t.Exception != null)
            {
                foreach (var inEx in t.Exception.Flatten().InnerExceptions)
                {
                    HandleException(inEx, level);
                }
            }
        }
    }
}
