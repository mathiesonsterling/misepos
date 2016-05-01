using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Entities.People
{
    public class User : EntityBase, IUser
    {
        public PersonName Name { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    if (Name != null)
                    {
                        return Name.FirstName + " " + Name.LastName;
                    }
                    return string.Empty;
                }
                return _displayName;
            }
            set { _displayName = value; }
        }

        /// <summary>
        /// We only store the hash, since this field goes across the wire
        /// </summary>
        /// <value>The password hash.</value>
        public Password Password
        {
            get;
            set;
        }

        public EmailAddress PrimaryEmail
        {
            get;
            set;
        }

        public IList<EmailAddress> Emails
        {
            get;
            set;
        }

        public IEnumerable<EmailAddress> GetEmailAddresses()
        {
            var res = Emails == null ? new List<EmailAddress>() : new List<EmailAddress>(Emails);
            if (PrimaryEmail != null)
            {
                if (res.Select(e => e.Value).Contains(PrimaryEmail.Value) == false)
                {
                    res.Add(PrimaryEmail);
                }
            }

            return res.Where(e => e != null);
        }
    }
}
