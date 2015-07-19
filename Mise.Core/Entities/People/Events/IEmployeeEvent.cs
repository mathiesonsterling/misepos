using System;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;


namespace Mise.Core.Entities.People.Events
{
	/// <summary>
	/// Basic event base for anything to do with an employee
	/// </summary>
	public interface IEmployeeEvent : IEntityEventBase
	{
		/// <summary>
		/// Employee this applies to
		/// </summary>
		/// <value>The employee I.</value>
		Guid EmployeeID{ get; }

	}
}

