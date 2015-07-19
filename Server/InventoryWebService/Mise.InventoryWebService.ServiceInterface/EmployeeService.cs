using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;
using Employee = Mise.InventoryWebService.ServiceModelPortable.Responses.Employee;

namespace Mise.InventoryWebService.ServiceInterface
{
    public class EmployeeService : Service
    {
        private readonly IEmployeeRepository _empRepos;

        public EmployeeService(IEmployeeRepository empRepos)
        {
            _empRepos = empRepos;
        }

        public async Task<EmployeeResponse> Get(Employee request)
        {
            if (_empRepos.Loading)
            {
                Thread.Sleep(100);
                if (_empRepos.Loading)
                {
                    throw new HttpError(HttpStatusCode.ServiceUnavailable, "Server has not yet loaded");
                }
            }

            if (string.IsNullOrEmpty(request.Email) == false)
            {
                if (string.IsNullOrEmpty(request.PasswordHash))
                {
                    throw new ArgumentException("No Password provided!");
                }

                var results =
                    (await GetForEmailAndPass(request.Email, request.PasswordHash)).ToList();
                if (results.Any() && results.First() != null)
                {
                    return new EmployeeResponse
                    {
                        Results = results.Cast<Core.Common.Entities.Employee>()
                    };
                }
                throw HttpError.NotFound("No employee found for email " + request.Email);
            }

            if (request.EmployeeID.HasValue)
            {
                var emp = _empRepos.GetByID(request.EmployeeID.Value) as Core.Common.Entities.Employee;
                if (emp == null)
                {
                    throw HttpError.NotFound("No employee found for ID " + request.EmployeeID.Value);
                }
                emp = emp.Clone() as Core.Common.Entities.Employee;
                if (emp == null)
                {
                    throw HttpError.Conflict("Server error in clone");
                }
                emp.Password = null;
                return new EmployeeResponse
                {
                    Results = new List<Core.Common.Entities.Employee> {emp}
                };
            }

            if (request.RestaurantID.HasValue)
            {
                var emps =  _empRepos.GetAll()
                    .Where(e => e.GetRestaurantIDs().Contains(request.RestaurantID.Value))
                    .Cast<Core.Common.Entities.Employee>()
                    .Select(e => e.Clone() as Core.Common.Entities.Employee)
                    .ToList();

                //we don't want to publish employee passwords unless they give us the hash, so blank them
                foreach (var emp in emps)
                {
                    emp.Password = null;
                }

                return new EmployeeResponse
                {
                    Results = emps
                };
            }

            throw new ArgumentException("No data given for request");
        }

        private async Task<IEnumerable<IEmployee>> GetForEmailAndPass(string rawEmail, string rawPass)
        {
            var password = new Password {HashValue = rawPass};
            var email = new EmailAddress {Value = rawEmail};
            var emp = await _empRepos.GetByEmailAndPassword(email, password);

            return new List<IEmployee> {emp};
        }
    }
}
