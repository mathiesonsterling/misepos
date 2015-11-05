using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;

namespace Mise.Core.Server.Windows.DataObjects
{
    public class AzureEventStorage : EntityData
    {
        public string MiseEventType { get; set; }
        public Guid EventID { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public string JSON { get; set; }
    }
}
