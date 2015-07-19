using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Services;

namespace MiseInventoryService
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message, LogLevel level = LogLevel.Error)
        {
            Console.WriteLine(level + " :  " + message);
            Trace.WriteLine(level+ ":" + message);
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
            Log(message, LogLevel.Error);
        }

        public void Fatal(string message)
        {
            Log(message, LogLevel.Fatal);
        }

        public void HandleException(Exception ex, LogLevel level = LogLevel.Error)
        {
            Log("Exception :: " + level + "::" + ex.StackTrace);
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
