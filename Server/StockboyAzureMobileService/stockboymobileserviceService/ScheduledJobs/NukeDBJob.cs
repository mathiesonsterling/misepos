using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using stockboymobileserviceService.Models;

namespace stockboymobileserviceService.ScheduledJobs
{
    public class NukeDBJob : ScheduledJob
    {
        public override Task ExecuteAsync()
        {
            var dbContext = new stockboymobileserviceContext();

            dbContext.Database.Initialize(false);

            return Task.FromResult(true);
        }
    }
}
