using System;
using System.Threading.Tasks;

using Mise.Core.ValueItems;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant;
namespace Mise.Inventory
{
	public class RegistrationService : IRegistrationService
	{
		public RegistrationService()
		{
			
		}

		#region IRegistrationService implementation

		public Task<IEmployee> RegisterEmployee(EmailAddress email, Password password, IRestaurant restaurant)
		{
			throw new NotImplementedException();
		}

		public Task<IEmployee> RegisterEmployee(EmailAddress email, Password password, string restaurantName, Location location)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

