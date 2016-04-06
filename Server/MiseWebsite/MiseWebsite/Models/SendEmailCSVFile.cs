using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiseWebsite.Models
{
    [Table("SendEmailCSVFile", Schema = "stockboymobile")]
    public class SendEmailCSVFile
    {
        public string Id { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// The ID of the item that needs to be sent
        /// </summary>
        public Guid EntityId { get; set; }

        public string MiseEntityType { get; set; }

        public bool Sent { get; set; }
    }
}