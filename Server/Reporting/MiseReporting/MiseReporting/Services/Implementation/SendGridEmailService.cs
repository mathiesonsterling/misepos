using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using SendGrid;

namespace MiseReporting.Services.Implementation
{
    public class SendGridEmailService : ISendEmailService
    {
        private const string SMTPUser = "smtp.sendgrid.net";
        private const string PASSWORD = "w759ySt8QLB33UR";
        private const string USERNAME = "azure_e928cf81dea1f11f14eea0cf09d1abf7@azure.com";

        public async Task SendEmail(IEnumerable<EmailAddress> to, EmailAddress @from, string subject, string body, 
            IEnumerable<byte[]> attachments)
        {
            var message = new SendGridMessage
            {
                Text = body,
                Subject = subject,
                From = new MailAddress(@from.Value)
            };
            message.AddTo(to.Select(e => e.Value));

            var attachStreams = attachments.Select(a => new MemoryStream(a));
            foreach (var attachStream in attachStreams)
            {
                message.AddAttachment(attachStream, "inventory.csv");
            }

            var credentials = new NetworkCredential(USERNAME, PASSWORD);
            var transportWeb = new Web(credentials);

            await transportWeb.DeliverAsync(message);
        }

    }
}
