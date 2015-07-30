using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Services.UtilityServices
{
    public interface IErrorTrackingService
    {
        void ReportException(Exception e, LogLevel level);

        /// <summary>
        /// Identify a user via some attributes
        /// </summary>

        void Identify(Guid? userID, EmailAddress email, PersonName name, string deviceID, bool isAnonymous);
    }
}
