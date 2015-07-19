using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Menu;
using Mise.Core.Repositories;
using Mise.Core.Services;
using ServiceStack;

namespace Mise.RestaurantServer.ServiceStack
{
    [Route("/api/{RestaurantFriendlyName}/Menu/{MenuName*}", "GET")]
    public class MenuRequest : IReturn<IEnumerable<Menu>>
    {
        [ApiMember(IsRequired = true)]
        public string RestaurantFriendlyName { get; set; }

        public string MenuName { get; set; }
    }

    public class MenuService : BaseMiseService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuRulesRepository _menuRulesRepository;
        public MenuService(IRestaurantRepository restaurantRepository, IMenuRepository menuRepository, IMenuRulesRepository menuRulesRepository, ILogger logger) : base(restaurantRepository, logger)
        {
            _menuRepository = menuRepository;
            _menuRulesRepository = menuRulesRepository;
        }

        public object Get(MenuRequest request)
        {
            var restaurant = GetRestaurantByFriendlyName(request.RestaurantFriendlyName);

            var res = _menuRepository.GetAll().Where(m => m.RestaurantID == restaurant.ID);
            if (string.IsNullOrEmpty(request.MenuName) == false)
            {
                res = res.Where(m => m.Name == request.MenuName);
            }
            return res;
        }
    }
}