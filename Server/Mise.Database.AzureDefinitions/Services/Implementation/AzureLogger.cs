using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;

namespace Mise.Database.AzureDefinitions.Services.Implementation
{
    public class AzureLogger : ILogger
    {
        public void Log(string message, LogLevel level = LogLevel.Error)
        {
            throw new NotImplementedException();
        }

        public void Debug(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string message)
        {
            throw new NotImplementedException();
        }

        public void HandleException(Exception ex, LogLevel level = LogLevel.Error)
        {
            throw new NotImplementedException();
        }

        public void HandleFaultedTask(Task t, LogLevel level = LogLevel.Error)
        {
            throw new NotImplementedException();
        }
    }
}
