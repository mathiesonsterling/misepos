using System.Collections.Generic;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.People
{
    public class Customer : RestaurantEntityBase
    {
        public Customer()
        {
        }

        public Customer(CreditCard card)
        {
            Name = card.Name;
        }

        public int PersonID
        {
            get;
            set;
        }

        public PersonName Name;

		public EmailAddress PrimaryEmail {
			get;
			set;
		}

		public IEnumerable<EmailAddress> Emails {
			get;
			set;
		}
    }
}
