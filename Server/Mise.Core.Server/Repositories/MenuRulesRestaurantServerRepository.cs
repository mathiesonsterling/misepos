using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Common.Repositories;
using Mise.Core.Entities.Menu;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Common.Repositories.Base;
namespace Mise.Core.Server.Repositories
{
    public class MenuRulesRestaurantServerRepository : BaseRestaurantServerRepository<MenuRule>, IMenuRulesRepository
    {
        private readonly IMiseAdminServer _server;
        private readonly IRestaurantServerDAL _dal;
        private readonly ILogger _logger;
        public MenuRulesRestaurantServerRepository(IMiseAdminServer server, IRestaurantServerDAL dal, ILogger logger, Guid? restaurantID) : base(dal, logger, restaurantID)
        {
            _server = server;
            _dal = dal;
            _logger = logger;
        }

        public void Load(Guid? restaurantID)
        {
            List<MenuRule> items;
            try
            {
                items = restaurantID.HasValue ? _server.GetMenuRules(restaurantID.Value).ToList() : _server.GetAllMenuRules().ToList();
            }
            catch (Exception e)
            {
                _logger.HandleException(e);
                _logger.Log("Unable to retrieve from server, pulling from DB");
                items = _dal.GetEntitiesAsync<MenuRule>(restaurantID.HasValue ? restaurantID.Value : Guid.Empty).Result.ToList();
            }
            if (items != null)
            {
				Cache.UpdateCache (items);
                var task = _dal.UpsertEntitiesAsync(items);
                task.Wait();
            }
        }
    }
}
