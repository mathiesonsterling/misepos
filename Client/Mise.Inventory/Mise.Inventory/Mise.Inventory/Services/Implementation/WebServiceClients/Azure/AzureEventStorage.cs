using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mise.Core.Common.Events.DTOs;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.MobileServices;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
    public class AzureEventStorage
    {
        public AzureEventStorage() { }

        public AzureEventStorage(EventDataTransportObject dto)
        {
            MiseEventType = dto.EventType.ToString();
            EventID = dto.Id;
            id = dto.Id.ToString();
            EventDate = dto.CreatedDate;
            JSON = dto.JSON;
        }

        public string id { get; set; }
        public string MiseEventType { get; set; }
        public Guid EventID { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string JSON { get; set; }
		[JsonProperty(PropertyName = "__version")]
		//[Version]
		public string Version { set; get; }
    }
}
