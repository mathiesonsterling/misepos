using System;
using System.Threading.Tasks;

namespace Mise.Core.Services.UtilityServices
{
    /// <summary>
    /// Represents any service which allows us to log events
    /// </summary>
    public interface ILogger
    {
		void Log(string message, LogLevel level = LogLevel.Error);

		void Debug(string message);
		void Info(string message);
		void Warn(string message);
		void Error(string message);
		void Fatal(string message);

		void HandleException(Exception ex, LogLevel level = LogLevel.Error);

        void HandleFaultedTask(Task t, LogLevel level = LogLevel.Error);
    }
}
