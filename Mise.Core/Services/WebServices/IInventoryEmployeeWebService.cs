using Mise.Core.Services.WebServices;
using Mise.Core.Entities.People.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities.People;
using System;
using Mise.Core.ValueItems;


namespace Mise.Core.Services.WebServices
{
	public interface IInventoryEmployeeWebService : IEventStoreWebService<IEmployee, IEmployeeEvent>
	{
		/// <summary>
		/// All employees (for a restaurant)
		/// </summary>
		/// <returns></returns>
		Task<IEnumerable<IEmployee>> GetEmployeesAsync ();

		/// <summary>
		/// Get all the employees that work at a restaurant
		/// </summary>
		/// <returns>The employees for restaurant.</returns>
		/// <param name="restaurantID">Restaurant I.</param>
		Task<IEnumerable<IEmployee>> GetEmployeesForRestaurant (Guid restaurantID);

		/// <summary>
		/// Gets the employee by primary email, if they exist
		/// </summary>
		/// <returns>The employee by primary email.</returns>
		/// <param name="email">Email.</param>
		/// <param name = "password"></param>
		Task<IEmployee> GetEmployeeByPrimaryEmailAndPassword (EmailAddress email, Password password);
	}
}

