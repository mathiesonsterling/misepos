using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeveloperTools.Commands
{
    public abstract class BaseProgressReportingCommand
    {
        public abstract Task Execute();
        /// <summary>
        /// Number of reporting steps we have
        /// </summary>
        public abstract int NumSteps { get; }

        private readonly IProgress<ProgressReport> _progress;
        private int _currentStep;

        protected BaseProgressReportingCommand(IProgress<ProgressReport> progress)
        {
            _progress = progress;
        }

        protected void Report(string message)
        {
            var current = _currentStep++;
            if (current > NumSteps)
            {
                current = NumSteps;
            }
            _progress.Report(new ProgressReport
            {
                CurrentProgressAmount = current,
                TotalProgressAmount = NumSteps,
                CurrentProgressMessage = message
            });
        }

        protected void Finish()
        {
            _progress.Report(new ProgressReport
            {
                CurrentProgressAmount = NumSteps,
                TotalProgressAmount = NumSteps,
                CurrentProgressMessage = "Complete"
            });
        }
    }
}
