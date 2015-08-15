using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiseReporting.Repository
{
    /// <summary>
    /// Todo - replace this with DI
    /// </summary>
    static class RepositoryFactory
    {
        public static Guid TestRestGUID = Guid.NewGuid();
        public static Guid TestInvGuid = Guid.NewGuid();
    }
}
