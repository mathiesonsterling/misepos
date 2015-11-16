using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace stockboymobileserviceService.Services.Implementation
{
    public class StripePaymentProviderService : IPaymentProviderService
    {
        public Task CreateAccountFromToken(CreditCard card, MisePaymentPlan plan)
        {
            throw new NotImplementedException();
        }
    }
}
