using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Mise.Core.Common.Entities;
using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;

namespace Mise.RestaurantServer.Controllers
{
    public class RestaurantsController : ApiController
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly ICheckRepository _checkRepository;
        private readonly IJSONSerializer _serializer;
        private readonly ILogger _logger;

        public RestaurantsController(IRestaurantRepository restaurantRepository, ICheckRepository checkRepository, IJSONSerializer jsonSerializer, ILogger logger)
        {
            _restaurantRepository = restaurantRepository;
            _checkRepository = checkRepository;
            _serializer = jsonSerializer;
            _logger = logger;
        }

      
        // GET: api/Restaurant
        /// <summary>
        /// TODO - lock this down to only the restuarants under an account!
        /// </summary>
        /// <returns></returns>
        [Route("api/restaurants")]
        [HttpGet]
        public IEnumerable<IRestaurant> GetAllRestaurants()
        {
            return _restaurantRepository.GetAll();
        }


        [HttpGet]
        [Route("api/restaurants/{restaurantName}")]
        public IHttpActionResult GetRestaurant(string restaurantName)
        {
           var item = _restaurantRepository.GetAll().FirstOrDefault(r => r.FriendlyID == restaurantName);
            if (item != null)
            {
                return Ok(item);
            }
            return NotFound();
        }
			

        #region Terminals
        [Route("api/restaurants/{restaurantName}/{terminalName}")]
        [Route("api/restaurants/{restaurantName}/terminals/{terminalName}")]
        [HttpGet]
        public IHttpActionResult GetTerminalByName(string restaurantName, string terminalName)
        {
            var restaurant = _restaurantRepository.GetByFriendlyID(restaurantName);
            if (restaurant == null)
            {
                _logger.Log("Cannot find restaurant with friendly id of " + restaurantName, LogLevel.Info);
                return NotFound();
            }
            if (terminalName == "terminals")
            {
                return Ok(restaurant.GetTerminals());
            }
            var term = restaurant.GetTerminals().FirstOrDefault(t => t.FriendlyID == terminalName);
            if (term == null)
            {
                return NotFound();
            }

            return Ok(term);
        }


        /// <summary>
        /// Allows a terminal to give us their device ID.  We then add the terminal to the restaurant, and 
        /// they can repoll for it
        /// </summary>
        /// <param name="restaurantName"></param>
        /// <param name="terminalInfo"></param>
        /// <returns></returns>
        [Route("api/restaurants/{restaurantName}/terminals")]
        [HttpPut]
        public IHttpActionResult RegisterTerminal(string restaurantName, [FromBody]string terminalInfo)
        {
			throw new NotImplementedException ();
        }

        [Route("api/restaurants/{restaurantName}/terminals/{terminalName}")]
        [HttpDelete]
        public IHttpActionResult DeRegisterTerminal(string restaurantName, string machineIdentifier)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Checks

        [Route("api/restaurants/{restaurantName}/checks/open")]
        [HttpGet]
        public IEnumerable<ICheck> GetOpenChecksForRestaurant(string restaurantName)
        {
            return _checkRepository.GetOpenChecks(null);
        }

        [Route("api/restaurants/{restaurantName}/checks")]
        [HttpGet]
        public IEnumerable<ICheck> GetAllChecksForRestaurant(string restaurantName)
        {
            return _checkRepository.GetChecksPriorToZ();
        }

        [Route("api/restaurants/{restaurantName}/checks/{checkID}")]
        [HttpGet]
        public IHttpActionResult GetCheckByID(string restaurantName, Guid checkID)
        {
            var check = _checkRepository.GetByID(checkID);
            if (check == null)
            {
                return NotFound();
            }
            return Ok(check);
        }

        [HttpPut]
        [Route("api/restaurants/{restuarantName}/checks")]
        public IHttpActionResult AddCheckEvents(string restaurantName, [FromBody] string checkEventsJSON )
        {
            var events = _serializer.Deserialize<IEnumerable<ICheckEvent>>(checkEventsJSON);
            if (events == null)
            {
                return BadRequest("Invalid JSON in CheckEvents");
            }

            var res = _checkRepository.ApplyEvents(events);
            if (res != null)
            {
                return Ok(res);
            }

            _logger.Log("Error while applying check events!");
            return InternalServerError();
        }
        #endregion
    }
}
