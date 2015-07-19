using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Services
{
    public interface IDeviceLocationService
    {
        /// <summary>
        /// Gets where the device running our applicaiton is in the world
        /// </summary>
        /// <returns></returns>
        Task<Location> GetDeviceLocation();
    }
}
