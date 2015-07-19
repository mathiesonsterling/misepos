using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mise.Core.Server.Services;

namespace MiseInventoryService
{
    public class IISHostingEnvironment : IWebHostingEnvironment
    {
        public void QueueBackgroundWorkItem(Action<CancellationToken> workItem)
        {
            System.Web.Hosting.HostingEnvironment.QueueBackgroundWorkItem(workItem);
        }
    }
}
