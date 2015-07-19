using System;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    [Route("/restaurants/{RestaurantID}/employees")]
    [Route("/restaurants/{RestaurantID}/employees/{EmployeeID}")]
    [Route("/employees/{Email}/{PasswordHash}")]
    public class Employee : IReturn<EmployeeResponse>
    {
        public Guid? EmployeeID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string Email { get; set; }

        //TODO should this be sent?
        public string PasswordHash { get; set; }

    }
}