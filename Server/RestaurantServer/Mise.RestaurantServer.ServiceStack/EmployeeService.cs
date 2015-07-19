using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using ServiceStack;
using Mise.Core.Common.Services.DAL;
using Mise.Core.ValueItems;

namespace Mise.RestaurantServer.ServiceStack
{
    [Route("/api/{RestaurantFriendlyName}/Employees/{EmployeeID*}", "GET")]
    public class EmployeeRequest : IReturn<IEnumerable<IEmployee>>
    {
        [ApiMember(IsRequired = true)]
        public string RestaurantFriendlyName { get; set; }

        /// <summary>
        /// If set, we only want the employee with the given ID
        /// </summary>
        public Guid? EmployeeID { get; set; }

        /// <summary>
        /// If set, we only want employees with matching status
        /// </summary>
        public bool? ClockedIn { get; set; }
    }

    [Route("/api/{RestaurantFriendlyName}/Employees/", "POST")]
    public class EmployeeEventsRequest : IReturnVoid
    {
        [ApiMember(IsRequired = true)]
        public string RestaurantFriendlyName { get; set; }
        [ApiMember(IsRequired = true)]
        public EventDataTransportObject[] Events { get; set; } 
    }

    public class EmployeeService : BaseMiseService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRestaurantServerDAL _dal;
        private readonly EventDataTransportObjectFactory _dtoFactory;
        public EmployeeService(IRestaurantRepository restaurantRepository, IEmployeeRepository employeeRepository, IRestaurantServerDAL dal, IJSONSerializer serailizer, ILogger logger) : base(restaurantRepository, logger)
        {
            _employeeRepository = employeeRepository;
            _dal = dal;
            _dtoFactory = new EventDataTransportObjectFactory(serailizer);
        }

        public void Post(EmployeeEventsRequest request)
        {
            //get our restaurant
            if (request.Events == null || request.Events.Any() == false)
            {
                throw new ArgumentException("No events were given in request!");
            }

            var realEvents = request.Events.Select(dto => _dtoFactory.ToEmployeeEvent(dto)).ToList();

            //check our restaurantID is valid?

            //upsert out entities
            var task = _dal.StoreEventsAsync(realEvents);
            task.Wait();
            var res = task.Result;

            if (res == false)
            {
                throw new Exception("Error when storing events ");
            }

            //group by our check ID, then update the checks
            var groupedEvents = realEvents.GroupBy(e => e.EmployeeID);
            var employees = (from @group in groupedEvents 
                          select @group.Select(t => t) 
                          into events 
                          select _employeeRepository.ApplyEvents(events)).ToList();

            foreach (var emp in employees)
            {
                _employeeRepository.Commit(emp.ID);
            }
            var upsertTask = _dal.UpsertEntitiesAsync(employees);
            upsertTask.Wait();
        }

        public object Get(EmployeeRequest request)
        {
            var restaurant = GetRestaurantByFriendlyName(request.RestaurantFriendlyName);

            IEnumerable<IEmployee> res;
            if (request.EmployeeID.HasValue)
            {
                var foundEmp = _employeeRepository.GetByID(request.EmployeeID.Value);
                if (foundEmp == null)
                {
                    throw new ArgumentException("No employee found with ID " + request.EmployeeID.Value);
                }
                res = new[] {foundEmp};
            }
            else
            {
				res = _employeeRepository.GetAll().Where(e => e.GetRestaurantIDs().Contains(restaurant.ID));
            }

            if (request.ClockedIn.HasValue)
            {
                res = res.Where(e => e.CurrentlyClockedInToPOS == request.ClockedIn.Value);
            }

            return res;
        }
    }
}