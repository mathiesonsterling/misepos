using System.Collections.Generic;
using System;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
namespace Mise.Core.Repositories
{
    public interface ICheckRepository : IEventSourcedEntityRepository<ICheck, ICheckEvent>
    {
        IEnumerable<ICheck> GetOpenChecks(IEmployee employee);

		IEnumerable<ICheck> GetClosedChecks ();

        /// <summary>
        /// Gets all checks, including the closed ones, that have not yet been Z'd
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICheck> GetChecksPriorToZ();
   
    }
}
