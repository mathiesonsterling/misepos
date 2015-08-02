using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Common.Events.DTOs.AzureTypes
{
    public class AzureEventStorage
    {
        public AzureEventStorage() { }

        public AzureEventStorage(EventDataTransportObject dto)
        {
            MiseEventType = dto.EventType.ToString();
            EventID = dto.ID;
            id = dto.ID.ToString();
            EventDate = dto.CreatedDate;
            JSON = dto.JSON;
        }

        public string id { get; set; }
        public string MiseEventType { get; set; }
        public Guid EventID { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string JSON { get; set; }
    }
}
