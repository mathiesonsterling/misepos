using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.People;
using Newtonsoft.Json;
namespace Mise.Core.Client.Entities.People
{
    public class Employee : BaseUserDbEntity<IEmployee, Core.Common.Entities.People.Employee>
    {
	    public Employee(){}

	    public Employee(IEmployee source, ICollection<Restaurant.Restaurant> restaurantsWorkingAt) :base(source)
	    {
		    LastDeviceIDLoggedInWith = source.LastDeviceIDLoggedInWith;
		    LastTimeLoggedIntoInventoryApp = source.LastTimeLoggedIntoInventoryApp;
		    CurrentlyLoggedIntoInventoryApp = source.CurrentlyLoggedIntoInventoryApp;
	        RestaurantsEmployedAt = restaurantsWorkingAt.Select(r => new EmployeeRestaurantRelationships(this, r)).ToList();
	        var restIds = restaurantsWorkingAt.Select(r => r.RestaurantID);
            RestaurantsEmployedAtIds = IDListToString(restIds);
	    }

        public DateTimeOffset? LastTimeLoggedIntoInventoryApp { get; set; }

        public string LastDeviceIDLoggedInWith { get; set; }

        public bool CurrentlyLoggedIntoInventoryApp { get; set; }

        [JsonIgnore]
        public List<EmployeeRestaurantRelationships> RestaurantsEmployedAt { get; set; } 

        public string RestaurantsEmployedAtIds { get; set; }

        protected override Core.Common.Entities.People.Employee CreateConcretePerson()
        {
            return new Core.Common.Entities.People.Employee
            {
                RestaurantsAndAppsAllowed = GenerateAppAndRestaurantDictionary(),
                CurrentlyLoggedIntoInventoryApp = CurrentlyLoggedIntoInventoryApp,
                LastDeviceIDLoggedInWith = LastDeviceIDLoggedInWith,
                LastTimeLoggedIntoInventoryApp = LastTimeLoggedIntoInventoryApp,
            };
        }

        private IDictionary<Guid, IList<MiseAppTypes>> GenerateAppAndRestaurantDictionary()
        {
            if (RestaurantsEmployedAtIds == null)
            {
                return new Dictionary<Guid, IList<MiseAppTypes>>();
            }

            var ids = StringToIDList(RestaurantsEmployedAtIds);

            return ids.ToDictionary<Guid, Guid, IList<MiseAppTypes>>(restId => restId, restId => new List<MiseAppTypes> {MiseAppTypes.StockboyMobile});
            /*
            if (RestaurantsEmployedAt == null)
            {
                return new Dictionary<Guid, IList<MiseAppTypes>>();
            }

            var dic = new Dictionary<Guid, IList<MiseAppTypes>>();
            foreach (var rest in RestaurantsEmployedAt.Select(r => r.Restaurant))
            {
                IList<MiseAppTypes> apps = rest.RestaurantApplicationUses.Select(ua => (MiseAppTypes) ua.MiseApplication.AppTypeValue).ToList();
                if (apps.Any())
                {
                    dic.Add(rest.RestaurantID, apps);
                }
            }
            return dic;*/
        }
    }
}
