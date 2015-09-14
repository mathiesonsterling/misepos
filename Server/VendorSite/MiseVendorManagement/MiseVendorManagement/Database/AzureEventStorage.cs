using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiseVendorManagement.Database
{
    [Table("stagingstockboymobileservice.AzureEventStorages")]
    public partial class AzureEventStorage
    {
        public string Id { get; set; }

        public string BuildLevel { get; set; }

        public string MiseEventType { get; set; }

        public Guid EventID { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string JSON { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] Version { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }
    }
}
