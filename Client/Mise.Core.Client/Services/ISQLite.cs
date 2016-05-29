using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Client.Services
{
    /// <summary>
    /// Provides platform specific information on constructing the SQLite DB
    /// </summary>
    public interface ISQLite
    {
        string GetLocalFilename();

        Task DeleteDatabaseFile();
    }
}
