using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.People;

namespace MiseReporting.Models
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public IEnumerable<RestaurantViewModel> RestaurantsWorkedAt { get; set; } 

        public IEnumerable<RestaurantViewModel> PossibleRestaurants { get; set; }
         
        public string RestaurantsDisplay { get { return string.Join(",", RestaurantsWorkedAt.Select(r => r.Name)); } }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public EmployeeViewModel() { }

        public EmployeeViewModel(IEmployee emp, IEnumerable<RestaurantViewModel> restaurantsWorkedAt)
        {
            Id = emp.Id;
            if (emp.Name == null)
            {
                throw new ArgumentException("Name is not sent!");
            }
            FirstName = emp.Name.FirstName;
            MiddleName = emp.Name.MiddleName;
            LastName = emp.Name.LastName;

            Email = emp.PrimaryEmail.Value;

            RestaurantsWorkedAt = restaurantsWorkedAt;

            Password = emp.Password != null ? "********" : string.Empty;
        }
    }
}
