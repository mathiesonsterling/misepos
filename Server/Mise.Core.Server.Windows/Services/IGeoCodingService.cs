using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Server.Windows.Services
{
    public interface IGeoCodingService
    {
        Task<Location> GetLocationForAddress(StreetAddress address);
    }
}
