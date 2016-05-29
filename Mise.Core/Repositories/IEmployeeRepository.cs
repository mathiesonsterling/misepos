using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.ValueItems;


namespace Mise.Core.Repositories
{
    public interface IEmployeeRepository : IEventSourcedEntityRepository<IEmployee, IEmployeeEvent>
    {
		/// <summary>
		/// Get an employee by their POS passcode, if they've been loaded
		/// </summary>
		/// <returns>The by passcode.</returns>
       // IEmployee GetByPasscode(string passcode);

		/// <summary>
		/// Get an employee by their sign in credentials
		/// </summary>
		/// <returns>The by user name and password.</returns>
		/// <param name = "email"></param>
		/// <param name="password">Password.</param>
		Task<IEmployee> GetByEmailAndPassword (EmailAddress email, Password password);

		Task<bool> IsEmailRegistered (EmailAddress email);
    }
}
