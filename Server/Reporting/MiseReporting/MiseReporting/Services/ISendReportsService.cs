using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiseReporting.Services
{
    public interface ISendReportsService
    {
        Task SendCSVReportsForNewItems();
    }
}
