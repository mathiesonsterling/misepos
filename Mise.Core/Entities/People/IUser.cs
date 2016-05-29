using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.People
{
    public interface IUser : IPerson
    {
        /// <summary>
        /// Comparable hash of the employee's login password
        /// </summary>
        /// <value>The password hash.</value>
        Password Password { get; }
    }
}
