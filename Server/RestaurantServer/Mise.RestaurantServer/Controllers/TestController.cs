using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Mise.RestaurantServer.Controllers
{
    public class TestController : ApiController
    {
        // GET api/test
        public IEnumerable<string> Get()
        {
            var connString = ConfigurationManager.AppSettings["StorageConnectionString"];

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("events");
            table.CreateIfNotExists();

            CloudTable table2 = tableClient.GetTableReference("entities");
            table2.CreateIfNotExists();
            return new string[] { connString };
        }
    }
}
