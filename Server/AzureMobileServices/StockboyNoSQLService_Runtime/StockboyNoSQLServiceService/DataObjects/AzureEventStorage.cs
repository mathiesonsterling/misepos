using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace StockboyNoSQLServiceService.DataObjects
{
    public class AzureEventStorage : EntityData
    {
        public string MiseEventType { get; set; }
        public Guid EventID { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string JSON { get; set; }
    }
}