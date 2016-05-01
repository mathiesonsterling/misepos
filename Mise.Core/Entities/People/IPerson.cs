using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.People
{
    public interface IPerson
    {
        PersonName Name { get; }

        /// <summary>
        /// Allows us to override the employee's name for display on the POS
        /// </summary>
        string DisplayName { get; }


        /// <summary>
        /// Return all the emails a person has
        /// </summary>
        /// <returns></returns>
        IEnumerable<EmailAddress> GetEmailAddresses();


        EmailAddress PrimaryEmail { get; set; }
    }
}
