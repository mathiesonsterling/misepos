using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Entities.Base
{
    /// <summary>
    /// Represents an entity that can make a copy of itself
    /// </summary>
    public interface ICloneableEntity : IEntityBase
    {
        ICloneableEntity Clone();
    }
}
