using System;

namespace Mise.Core.Server
{
    public interface IConfig
    {
        Uri Neo4JConnectionDBUri { get; }
    }
}