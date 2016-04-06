using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiseReporting
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

        /*
        public bool Error { get; set; }
        public string SendError { get; set; }*/
    }
}
