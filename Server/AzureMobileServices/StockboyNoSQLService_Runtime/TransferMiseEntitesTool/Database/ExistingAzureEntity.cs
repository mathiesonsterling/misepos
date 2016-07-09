using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransferMiseEntitesTool.Database
{
    [Table("AzureEntityStorages", Schema = "stockboymobile")]
    public class ExistingAzureEntity
    {
        public string Id { get; set; }

        public string MiseEntityType { get; set; }

        public Guid EntityID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string EntityJSON { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        [Column(TypeName = "timestamp")]
        [Timestamp]
        public byte[] Version { get; set; }


        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public bool Deleted { get; set; }


    }
}
