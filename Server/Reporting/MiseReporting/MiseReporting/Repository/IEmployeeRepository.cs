using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities.People;

namespace MiseReporting.Repository
{
    public interface IEmployeeRepository
    {
        Task<IEmployee> GetById(Guid id);
    }
}
