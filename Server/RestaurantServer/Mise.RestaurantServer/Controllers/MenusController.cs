using System.Linq;
using System.Web.Http;
using Mise.Core.Entities.Menu;
using Mise.Core.Repositories;

namespace Mise.RestaurantServer.Controllers
{
    public class MenusController : ApiController
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuRulesRepository _menuRulesRepository;
        private readonly IRestaurantRepository _restaurantRepository;

        public MenusController(IMenuRepository menuRepository, IMenuRulesRepository menuRulesRepository, IRestaurantRepository restaurantRepository)
        {
            _menuRepository = menuRepository;
            _menuRulesRepository = menuRulesRepository;
            _restaurantRepository = restaurantRepository;
        }
        //get all menus

        //get current menu
        [HttpGet]
        [Route("api/menus/{restaurantName}/current")]
        public IHttpActionResult GetCurrentMenu(string restaurantName)
        {
            var restaurant = _restaurantRepository.GetByFriendlyID(restaurantName);
            if (restaurant == null)
            {
                return NotFound();
            }
            //get the rules we have
            var rules = _menuRulesRepository.GetAll();
            var matchingRule = rules
                .Where(r => r.RestaurantID == restaurant.ID)
                .Where(t => t.CurrentlyApplies())
                .OrderByDescending(r => r.CreatedDate)
                .FirstOrDefault();
            Menu menu;
            if (matchingRule == null)
            {
                //just give our latest
                menu = _menuRepository.GetAll()
                        .Where(m => m.RestaurantID == restaurant.ID)
                        .OrderByDescending(m => m.CreatedDate)
                        .FirstOrDefault();
            }
            else
            {
                menu = _menuRepository.GetAll().FirstOrDefault(mr => mr.ID == matchingRule.RestaurantID);
            }
            if (menu != null)
            {
                return Ok(menu);
            }
            return NotFound();

        }
    }
}
