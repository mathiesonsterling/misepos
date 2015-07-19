using System.Collections.Generic;
using Mise.Core.Common.Entities;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    public class EmployeeResponse
    {
        public IEnumerable<Core.Common.Entities.Employee> Results { get; set; }
    }
}
