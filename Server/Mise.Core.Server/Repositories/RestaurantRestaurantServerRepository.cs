using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Server.Repositories
{
	public class RestaurantRestaurantServerRepository : BaseRestaurantServerRepository<IRestaurant>, IRestaurantRepository
    {
	    public IRestaurant ApplyEvent(IRestaurantEvent empEvent)
	    {
	        throw new NotImplementedException();
	    }

	    public IRestaurant ApplyEvents(IEnumerable<IRestaurantEvent> events)
	    {
	        throw new NotImplementedException();
	    }

	    public Guid GetEntityID(IRestaurantEvent ev)
	    {
	        return ev.RestaurantID;
	    }

	    public Task<CommitResult> CommitOnlyImmediately(Guid entityID)
	    {
	        throw new NotImplementedException();
	    }

	    public void CancelTransaction(Guid entityID)
	    {
	        throw new NotImplementedException();
	    }

	    public bool StartTransaction(Guid entityID)
	    {
	        throw new NotImplementedException();
	    }

	    public bool Dirty { get; private set; }
	    public int GetNumberOfEventsInTransacitonForEntity(Guid entityID)
	    {
	        throw new NotImplementedException();
	    }


	    public IEnumerable<IRestaurant> GetByName(string name)
	    {
	        return GetAll().Where(r => r.Name.ContainsSearchString(name));
	    }

	    public IEnumerable<IRestaurant> GetRestaurantsEmployeeWorksAt(Guid employeeID)
	    {
	        throw new NotImplementedException();
	    }

	    public IEnumerable<IRestaurant> GetRestaurantsForAccount(Guid accountID)
	    {
	        throw new NotImplementedException();
	    }

	    public ICollection<IRestaurant> Create(ICollection<IRestaurant> restaurants)
		{
			throw new NotImplementedException();
		}
        public ICollection<IRestaurant> Update(ICollection<IRestaurant> restaurants)
		{
			throw new NotImplementedException();
		}

        private readonly IMiseAdminServer _server;
        private readonly IDAL _dal;
        public RestaurantRestaurantServerRepository(IMiseAdminServer server, 
            IRestaurantServerDAL dal, ILogger logger, Guid? restaurantID) : base(dal, logger, restaurantID)
        {
            _dal = dal;
            _server = server;
        }

        public Task Load(Guid? restaurantID)
        {
            IList<Restaurant> list;
            if (restaurantID.HasValue)
            {
				var restaurant = _server.GetRestaurantSnapshot(restaurantID.Value) as Restaurant;
                list = new[] {restaurant};
            }
            else
            {
                //load ALL our restaurants
                list = _server.GetAllRestaurants().OfType<Restaurant> ().ToList ();
            }

			Cache.UpdateCache (list);
           return _dal.UpsertEntitiesAsync(list);
        }

        public IRestaurant GetByFriendlyID(string friendlyID)
        {
            return GetAll().FirstOrDefault(r => r.FriendlyID == friendlyID);
        }
    }
}
