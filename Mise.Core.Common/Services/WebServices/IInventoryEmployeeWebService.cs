using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.WebServices
{
	public interface IInventoryEmployeeWebService : IEventStoreWebService<Employee, IEmployeeEvent>
	{
		/// <summary>
		/// All employees (for a restaurant)
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<Employee>> GetEmployeesAsync ();

		/// <summary>
		/// Get all the employees that work at a restaurant
		/// </summary>
		/// <returns>The employees for restaurant.</returns>
		/// <param name="restaurantID">Restaurant I.</param>
		Task<IEnumerable<Employee>> GetEmployeesForRestaurant (Guid restaurantID);

		/// <summary>
		/// Gets the employee by primary email, if they exist
		/// </summary>
		/// <returns>The employee by primary email.</returns>
		/// <param name="email">Email.</param>
		/// <param name = "password"></param>
		Task<Employee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password);

		Task<bool> IsEmailRegistered (EmailAddress email);
	}
}

