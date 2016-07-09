using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace MiseReporting.Services
{
    public interface ISendEmailService
    {
        Task SendEmail(IEnumerable<EmailAddress> to, EmailAddress @from, string subject, string body,
            IEnumerable<byte[]> attachments);
    }
}
