using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class ApplicationInvitationResponse
    {
        public IEnumerable<Core.Common.Entities.ApplicationInvitation> Results { get; set; } 
    }
}
