using System;
using System.Threading.Tasks;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.Repositories;
using System.Dynamic;

namespace Mise.Core
{
	/// <summary>
	/// Basic unit of work pattern for our system.  Holds transactions together across repositories
	/// </summary>
	/// <remarks>In POS, the basic unit of work (transaction scope) is Check, Employee, and Terminal
	/// One employee working at one terminal on a check at a time.
	/// Because PortableLibraries can't do timers, we DI this from whatever</remarks>
	public interface IUnitOfWork
	{
		ICheckRepository CheckRepository{get;set;}
		IEmployeeRepository EmployeeRepository{get;set;}

		/// <summary>
		/// Start our unit of work
		/// </summary>
		/// <param name="terminalID">Terminal I.</param>
		/// <param name="employee">Employee.</param>
		/// <param name="check">Check.</param>
		bool Start (Guid terminalID, IEmployee employee, ICheck check);

		/// <summary>
		/// We like what we did, and want to commit it
		/// </summary>
		/// <param name="terminalID">Terminal I.</param>
		/// <param name="employee">Employee.</param>
		/// <param name="check">Check.</param>
		Task Commit (Guid terminalID, IEmployee employee, ICheck check);

		/// <summary>
		/// We don't like it, get us back to the last commit
		/// </summary>
		/// <returns><c>true</c> if this instance cancel terminalID employee check; otherwise, <c>false</c>.</returns>
		/// <param name="terminalID">Terminal I.</param>
		/// <param name="employee">Employee.</param>
		/// <param name="check">Check.</param>
		Task Cancel (Guid terminalID, IEmployee employee, ICheck check);

		/// <summary>
		/// Lets us specify an action should be done once our repository is freed up
		/// Equivalent of node.js' OnNextTick
		/// </summary>
		/// <param name = "terminalID">Device ID</param>
		/// <param name="employee">Employee.</param>
		/// <param name="check">Check.</param>
		/// <param name="action">Action.</param>
		void DoWhenRepositoryIsAvailable (Guid terminalID, IEmployee employee, ICheck check, 
			Action<IEmployee, ICheck> action);
	}
}

