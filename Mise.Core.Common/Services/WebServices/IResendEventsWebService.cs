using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Entities.Base;

namespace Mise.Core.Common.Services.WebServices
{
    public interface IResendEventsWebService
    {
        /// <summary>
        /// Send events back to the webservice
        /// </summary>
        /// <param name="events"></param>
        /// <returns>If items resent successfully</returns>
        Task<bool> ResendEvents(ICollection<IEntityEventBase> events);
    }
}
