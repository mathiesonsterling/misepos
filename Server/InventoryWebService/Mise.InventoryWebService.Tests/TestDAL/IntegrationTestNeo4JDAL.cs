using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Server;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services.UtilityServices;
using Mise.Neo4J.Neo4JDAL;
using MiseInventoryService;

namespace Mise.InventoryService.Tests.TestDAL
{
    public class IntegrationTestDAL : Neo4JEntityDAL
    {

        private IntegrationTestDAL(IConfig config, ILogger logger) : base(config, logger)
        {
        }
    }
}
