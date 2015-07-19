using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Server;

namespace MiseInventoryService
{
    public class Config : IConfig
    {
        public Uri Neo4JConnectionDBUri
        {
            get
            {
                //get the value from the config file
                var val = ConfigurationManager.ConnectionStrings["Neo4JRemoteConnectionString"];
                return new Uri(val.ConnectionString);
            }
        }
    }
}
