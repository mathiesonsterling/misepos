using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Restaurant;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class RestaurantRepository : BaseAdminServiceRepository<IRestaurant, IRestaurantEvent>, IRestaurantRepository
    {
        public RestaurantRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdateRestaurantAsync, EntityDAL.AddRestaurantAsync);
        }

        protected override IRestaurant CreateNewEntity()
        {
            return new Restaurant();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            //TODO real restaurant here as well!
            return ev is PlaceholderRestaurantCreatedEvent;
        }

        public override Guid GetEntityID(IRestaurantEvent ev)
        {
            return ev.RestaurantID;
        }

        protected override async Task LoadFromDB()
        {
            //debug only!
            Thread.Sleep(10000);
            var restaurants = await EntityDAL.GetRestaurantsAsync();
            Cache.UpdateCache(restaurants);
        }

        public IRestaurant GetByFriendlyID(string friendlyID)
        {
            return GetAll().FirstOrDefault(r => r.FriendlyID == friendlyID);
        }

        public IEnumerable<IRestaurant> GetByName(string name)
        {
            return GetAll().Where(r => r.Name.ContainsSearchString(name));
        }

        public IEnumerable<IRestaurant> GetRestaurantsForAccount(Guid accountID)
        {
            return GetAll().Where(r => r.AccountID.HasValue && r.AccountID == accountID);
        }
    }
}
