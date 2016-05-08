using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.People;

namespace Mise.Database.AzureDefinitions.Entities.People
{
    public class Employee : BaseUserDbEntity<IEmployee, Core.Common.Entities.People.Employee>
    {
        /// <summary>
        /// Employees may or may not have a restaurant at all times
        /// </summary>
        public Dictionary<Guid, List<MiseAppTypes>> RestaurantsAndAppsAllowed { get; set; }

        public DateTimeOffset? LastTimeLoggedIntoInventoryApp { get; set; }

        public string LastDeviceIDLoggedInWith { get; set; }

        public bool CurrentlyLoggedIntoInventoryApp { get; set; }

        protected override Core.Common.Entities.People.Employee CreateConcretePerson()
        {
            return new Core.Common.Entities.People.Employee
            {
                RestaurantsAndAppsAllowed = ConvertAppDictionary(RestaurantsAndAppsAllowed),
                CurrentlyLoggedIntoInventoryApp = CurrentlyLoggedIntoInventoryApp,
                LastDeviceIDLoggedInWith = LastDeviceIDLoggedInWith,
                LastTimeLoggedIntoInventoryApp = LastTimeLoggedIntoInventoryApp,
            };
        }

        private static IDictionary<Guid, IList<MiseAppTypes>> ConvertAppDictionary(Dictionary<Guid, List<MiseAppTypes>> source)
        {
            return source.ToDictionary<KeyValuePair<Guid, List<MiseAppTypes>>, Guid, IList<MiseAppTypes>>(kv => kv.Key, kv => kv.Value);
        }
    }
}
