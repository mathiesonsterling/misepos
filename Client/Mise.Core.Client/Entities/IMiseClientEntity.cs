using System;

namespace Mise.Core.Client.Entities
{
    public interface IMiseClientEntity
    {
        Guid EntityId{get;}
        bool Deleted{get;}
    }
}

