using System;

namespace Mise.Database.StorableEntities
{
    /// <summary>
    /// Base class for items which are going to be stored in the graph database as a node
    /// </summary>
    public interface IStorableEntityGraphNode
    {
        Guid ID { get; set; }
        string Revision { get; set; }

    }
}
