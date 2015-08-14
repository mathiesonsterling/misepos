using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories;
using Mise.Core.Entities.Menu;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Server.Repositories
{
    public class MenuRestaurantServerRepository : BaseRestaurantServerRepository<Menu>, IMenuRepository
    {
        private readonly IMiseAdminServer _server;
        private readonly IRestaurantServerDAL _dal;
        public MenuRestaurantServerRepository(IMiseAdminServer server, IRestaurantServerDAL dal, ILogger logger, Guid? restaurantID) : base(dal, logger, restaurantID)
        {
            _server = server;
            _dal = dal;
        }

        public async Task Load()
        {
            var menus = RestaurantID.HasValue
                ? await _server.GetMenus(RestaurantID.Value)
                : await _server.GetAllMenus();
            var menuL = menus.ToList();
			Cache.UpdateCache (menus);

            //use to list to block till we're loaded?
            var dalTask =  _dal.UpsertEntitiesAsync(menuL);

            Task.WaitAll(dalTask);
        }

        public Menu GetCurrentMenu()
        {
            //get our menu
            throw new NotImplementedException("Current is not set on RestaurantServer!");
        }
    }
}
