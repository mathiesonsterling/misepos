using System;

namespace Mise.Core.Common.Services.WebServices
{

    public class SendEventsException : Exception
    {
        public SendEventsException(string reasonPhrase)
            : base(reasonPhrase)
        {
            switch (reasonPhrase)
            {
                case "EmailAlreadyInUse":
                    Error = SendErrors.EmailAlreadyInUse;
                    break;
                case "EmailLoggedInElsewhere":
                    Error = SendErrors.EmailLoggedInElsewhere;
                    break;
                case "VendorAlreadyExists":
                    Error = SendErrors.VendorAlreadyExists;
                    break;
                default:
                    Error = SendErrors.Other;
                    break;
            }
        }
        public SendErrors Error { get; set; }
    }
    
}
