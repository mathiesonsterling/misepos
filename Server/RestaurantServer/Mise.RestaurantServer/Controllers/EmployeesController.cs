using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;

namespace Mise.RestaurantServer.Controllers
{
    public class EmployeesController : ApiController
    {
        private IEmployeeRepository EmployeeRepository { get; set; }
        private IRestaurantRepository RestaurantRepository { get; set; }
        private IJSONSerializer Serializer { get; set; }
        private ILogger Logger { get; set; }

        public EmployeesController(IEmployeeRepository employeeRepository, IRestaurantRepository restaurantRepository, IJSONSerializer serializer, ILogger logger)
        {
            EmployeeRepository = employeeRepository;
            RestaurantRepository = restaurantRepository;
            Serializer = serializer;
            Logger = logger;
        }

        [HttpGet]
        public IEnumerable<IEmployee> GetAllEmployees()
        {
            return EmployeeRepository.GetAll();
        }

        [HttpGet]
        [Route("api/employees/{restaurantName}")]
        public IEnumerable<IEmployee> GetEmployeesForRestaurant(string restaurantName)
        {
            var restaurant = RestaurantRepository.GetByFriendlyID(restaurantName);
            if (restaurant == null)
            {
               throw new HttpResponseException(HttpStatusCode.NotFound);
            }

			return EmployeeRepository.GetAll().Where(e => e.GetRestaurantIDs().Contains(restaurant.ID));
        }
        
        [Route("api/employees/{restaurantName}/{employeeID}")]
        [HttpGet]
        public IHttpActionResult GetEmployee(string restaurantName, Guid employeeID)
        {
            var emp = EmployeeRepository.GetByID(employeeID);
            if (emp != null)
            {
                return Ok(emp);
            }
            return NotFound();
        }
    }
}
