using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server;

namespace Mise.Database.AzureDefinitions.Entities.People
{
    /// <summary>
    /// Shows that an employee works at a restaurant
    /// </summary>
    public class EmployeeRestaurantRelationships : EntityData
    {
        public EmployeeRestaurantRelationships()
        {
            IsCurrentEmployee = true;
        }

        public EmployeeRestaurantRelationships(Employee employee, Restaurant.Restaurant restaurant) : this()
        {
            Employee = employee;
            Restaurant = restaurant;
            IsCurrentEmployee = true;

            Id = employee.Id + ":" + restaurant.RestaurantID;
        }

        public Employee Employee { get; set; }

	    [ForeignKey("Employee")]
	    public string EmployeeId { get; set; }

        public Restaurant.Restaurant Restaurant { get; set; }
	    [ForeignKey("Restaurant")]
	    public string RestaurantId { get; set; }

        public bool IsCurrentEmployee { get; set; }
    }
}
