using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferMiseEntitesTool.Producers
{
    class ParProducer : BaseAzureEntitiesProducer
    {
        protected override string EntityTypeString => "Mise.Core.Common.Entities.Inventory.Par";
    }
}
