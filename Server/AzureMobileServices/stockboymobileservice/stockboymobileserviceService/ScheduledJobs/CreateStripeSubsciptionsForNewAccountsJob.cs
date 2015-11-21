using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;

namespace stockboymobileserviceService.ScheduledJobs
{
    public class CreateStripeSubsciptionsForNewAccountsJob : ScheduledJob
    {

        public override Task ExecuteAsync()
        {
            //TODO just call the MAnagement service to do this!
            throw new NotImplementedException();
        } 
    }
}
