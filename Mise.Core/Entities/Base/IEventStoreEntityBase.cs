using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Entities.Base
{
    /// <summary>
    /// Represents an entity which is modified through event sourcing
    /// </summary>
    public interface IEventStoreEntityBase<in T> : IEntityBase, ICloneableEntity where T:IEntityEventBase
    {
        void When(T entityEvent);
    }
}
