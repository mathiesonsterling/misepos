using System;
using System.Threading.Tasks;

using Mise.Core.Entities.People;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Restaurant;


namespace Mise.Inventory
{
	public interface IRegistrationService
	{
		Task<IEmployee> RegisterEmployee(EmailAddress email, Password password, IRestaurant restaurant);

		Task<IEmployee> RegisterEmployee(EmailAddress email, Password password, string restaurantName, Location location);
	}
}

