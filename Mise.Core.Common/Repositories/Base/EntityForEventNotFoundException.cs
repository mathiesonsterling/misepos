using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Common.Repositories.Base
{
    public class EntityForEventNotFoundException : ArgumentException
    {
        public Guid EntityID { get; private set; }
        public EntityForEventNotFoundException(Guid entityID)
            : base("Event specified for entity " + entityID + " that can't be found")
        {
            EntityID = entityID;
        }
    }
}
