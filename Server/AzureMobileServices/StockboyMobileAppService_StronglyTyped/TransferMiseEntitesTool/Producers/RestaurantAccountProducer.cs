using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.DTOs;

namespace TransferMiseEntitesTool.Producers
{
    class RestaurantAccountProducer : BaseAzureEntitiesProducer
    {

        protected override string EntityTypeString => "Mise.Core.Common.Entities.Accounts.RestaurantAccount";
    }
}
