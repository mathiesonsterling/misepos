﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferMiseEntitesTool.Producers
{
    class ApplicationInvitationProducer : BaseAzureEntitiesProducer
    {
        protected override string EntityTypeString => "Mise.Core.Common.Entities.ApplicationInvitation";
    }
}
