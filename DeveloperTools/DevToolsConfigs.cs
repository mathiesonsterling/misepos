using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Server;

namespace DeveloperTools
{
    public class DevToolsConfigs : IConfig
    {
        public Uri Neo4JConnectionDBUri { get; set; }
    }
}
