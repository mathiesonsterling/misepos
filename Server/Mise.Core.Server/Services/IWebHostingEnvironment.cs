using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mise.Core.Server.Services
{
    /// <summary>
    /// Facade for items that the host gives us
    /// </summary>
    public interface IWebHostingEnvironment
    {
        void QueueBackgroundWorkItem(Action<CancellationToken> workItem);
    }
}
