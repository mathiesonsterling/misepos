using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Services
{
    public class RaygunErrorTracking : IErrorTrackingService
    {
        private RaygunClient _raygunClient;
        public RaygunErrorTracking()
        {
            _raygunClient = new RaygunClient();
        }
        public void ReportException(Exception e, LogLevel level)
        {
            if (_raygunClient == null)
            {
                _raygunClient = new RaygunClient();
            }
            _raygunClient.SendInBackground(e);
        }

        public void Identify(Guid? userID, EmailAddress email, PersonName name, string deviceID, bool isAnonymous)
        {
            var emailString = email != null ? email.Value : string.Empty;
            var message = new RaygunIdentifierMessage(emailString)
            {
                FirstName = name != null ? name.FirstName : string.Empty,
                FullName = name != null ? name.ToSingleString() : string.Empty,
                Identifier = userID.HasValue ? userID.Value.ToString() : string.Empty,
                IsAnonymous = isAnonymous,
                UUID = deviceID
            };

            _raygunClient.UserInfo = message;
        }
    }
}
