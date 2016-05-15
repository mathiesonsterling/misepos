﻿using System;
using System.Collections.Generic;
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
        }

        public Employee Employee { get; set; }
        public Restaurant.Restaurant Restaurant { get; set; }
        public bool IsCurrentEmployee { get; set; }
    }
}
