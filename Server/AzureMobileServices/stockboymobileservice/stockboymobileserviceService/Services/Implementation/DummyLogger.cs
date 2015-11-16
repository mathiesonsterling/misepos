using System;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;

namespace stockboymobileserviceService.Services.Implementation
{
    public class DummyLogger : ILogger
    {
        public void Log(string message, LogLevel level = LogLevel.Error)
        {
            Console.WriteLine(level + "::" +message);
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
            Log(ex.Message + ":::" + ex.StackTrace, level);
        }

        public void HandleFaultedTask(Task t, LogLevel level = LogLevel.Error)
        {
            if (t.Exception != null)
            {
                foreach (var ex in t.Exception.InnerExceptions)
                {
                    HandleException(ex);
                }
            }
        }
    }
}
