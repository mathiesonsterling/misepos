using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;

namespace Mise.InventoryWebService.ServiceInterface.Exceptions
{
    public class EmailAlreadyInUseException : Exception
    {
        public EmailAddress Email { get; private set; }

        public SendErrors SendError
        {
            get { return SendErrors.EmailAlreadyInUse; }
        }

        public EmailAlreadyInUseException(EmailAddress email)
            : base(SendErrors.EmailAlreadyInUse.ToString())
        {
            Email = email;
        }
    }
}
