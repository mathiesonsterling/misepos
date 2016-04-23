using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Mise.Core.ValueItems;
using MiseWebsite.Database;

namespace MiseWebsite.Services.Implementation
{
    public class MiseAccountService : IMiseAccountService
    {
        private readonly IManagementDAL _restaurantDAL;
        private readonly IAccountDAL _accountDAL;
        public MiseAccountService(IManagementDAL restaurantDAL, IAccountDAL accountDAL)
        {
            _restaurantDAL = restaurantDAL;
            _accountDAL = accountDAL;
        }

        public async Task<IEnumerable<MiseWebsiteAreas>> GetAreasUserHasAccessTo(EmailAddress email, Password password)
        {
            var allowedAreas = new List<MiseWebsiteAreas>();

            //check if they're a salesperson
            /*
            var salesPerson = await _accountDAL.GetResellerAccount(email, password);
            if (salesPerson != null)
            {
                allowedAreas.Add(MiseWebsiteAreas.Resellers);
            }*/

            //check for rest account
            var restAccount = await _restaurantDAL.GetAccount(email, password);
            if (restAccount != null)
            {
                allowedAreas.Add(MiseWebsiteAreas.Restaurants);
            }

            var employee = await _restaurantDAL.GetEmployee(email, password);
            if (employee != null)
            {
                allowedAreas.Add(MiseWebsiteAreas.Employee);
            }
            return allowedAreas;
        }
    }
}