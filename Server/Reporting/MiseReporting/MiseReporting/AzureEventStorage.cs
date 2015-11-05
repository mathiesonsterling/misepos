namespace MiseReporting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("stockboy.AzureEventStorages")]
    public class AzureEventStorage
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
