using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
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

        public IEnumerable<RestaurantViewModel> Restaurants{ get; set; }

        [DisplayName("Restaurants Worked At")]
        public string RestaurantsDisplay
        {
            get
            {
                return Restaurants != null ? string.Join(",", Restaurants.Select(r => r.Name)) : string.Empty;
            }
        }

        public IEnumerable<RestaurantViewModel> PossibleRestaurants { get; set; }

        public IEnumerable<Guid> PostedRestaurantGuids { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public EmployeeViewModel() { }

        public EmployeeViewModel(IEmployee emp, IEnumerable<RestaurantViewModel> restaurants)
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

            Restaurants = restaurants;
            PostedRestaurantGuids = new List<Guid>();
            Password = string.Empty;
        }
    }
}
