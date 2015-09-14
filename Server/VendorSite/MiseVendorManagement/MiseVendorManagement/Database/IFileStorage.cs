using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiseVendorManagement.Database
{
    public interface IFileStorage
    {
        Task StoreFile(string key, MemoryStream contents);
        Task<MemoryStream> RetrieveFile(string key);
        Task DeleteFile(string key);
    }
}
