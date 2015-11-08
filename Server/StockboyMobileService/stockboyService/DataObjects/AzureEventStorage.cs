using System;
using Microsoft.WindowsAzure.Mobile.Service;

namespace stockboyService.DataObjects
{
    public class AzureEventStorage : EntityData
    {
        public string MiseEventType { get; set; }
        public Guid EventID { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string JSON { get; set; }
    }
}
