using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MiseVendorManagement.Database
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly CloudBlobContainer _container;
        public AzureFileStorage()
        {
            var connString = "DefaultEndpointsProtocol=[https];AccountName=misestorage;AccountKey=zLYp26OA+s9tgjfGDzUAB1FRRWlsHFfuyO3y1s2GYqUDghvrmV5MXLtOxr4hj8ZHWnT8wkeLI6zl9V8tSDBA0A==";
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(connString);
            /*
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));*/

            // Create the blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            _container = blobClient.GetContainerReference("vendorimportcsvfiles");

            // Create the container if it doesn't already exist.
            _container.CreateIfNotExists();
        }

        public async Task StoreFile(string key, MemoryStream contents)
        {
            // Retrieve reference to a blob named "myblob".
            var blockBlob = _container.GetBlockBlobReference(key);

            // Create or overwrite the "myblob" blob with contents from a local file.
            await blockBlob.UploadFromStreamAsync(contents);
        }

        public async Task<MemoryStream> RetrieveFile(string key)
        {
            var blockBlob = _container.GetBlockBlobReference(key);

            var memStream = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(memStream);
            return memStream;
        }

        public Task DeleteFile(string key)
        {
            var blockBlob = _container.GetBlockBlobReference(key);

            return blockBlob.DeleteAsync();
        }
    }
}
