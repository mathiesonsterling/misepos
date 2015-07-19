using System;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Check.Events
{
    public interface ICheckEvent : IEntityEventBase
    {
        /// <summary>
        /// The check this applies to
        /// </summary>
        /// <value>The check I.</value>
        Guid CheckID{ get; }

        /// <summary>
        /// Employee who caused this event
        /// </summary>
        Guid EmployeeID { get; }
    }
}