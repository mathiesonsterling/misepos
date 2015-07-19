using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities.Check;
using Mise.Core.Repositories;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using ServiceStack;
using Mise.Core.Common.Services.DAL;

namespace Mise.RestaurantServer.ServiceStack
{
    [Route("/api/{RestaurantFriendlyName}/Checks/{CheckID*}", "GET")]
    public class CheckRequest : IReturn<IEnumerable<ICheck>>
    {
        [ApiMember(IsRequired = true)]
        public string RestaurantFriendlyName { get; set; }

        public Guid? CheckID { get; set; }

        public CheckPaymentStatus? PaymentStatus { get; set; }
    }

    [Route("/api/{RestaurantFriendlyName}/Checks/", "POST")]
    public class CheckEventRequest : IReturnVoid
    {
        [ApiMember(IsRequired = true)]
        public string RestaurantFriendlyName { get; set; }

        [ApiMember(IsRequired = true)]
        public List<EventDataTransportObject> Events { get; set; } 
    }

    public class CheckService : BaseMiseService
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IRestaurantServerDAL _dal;
        private readonly EventDataTransportObjectFactory _dtoFactory;
        public CheckService(IRestaurantRepository restaurantRepository, ICheckRepository checkRepository, IRestaurantServerDAL dal, IJSONSerializer serializer, ILogger logger) : base(restaurantRepository, logger)
        {
            _checkRepository = checkRepository;
            _dal = dal;
            _dtoFactory = new EventDataTransportObjectFactory(serializer);
        }

        public void Post(CheckEventRequest request)
        {
            //upsert out entities
            if (request.Events == null || request.Events.Any() == false)
            {
                throw new ArgumentException("No events were given in request!");
            }

            var realEvents = request.Events.Select(dto => _dtoFactory.ToCheckEvent(dto)).ToList();
            var storeInDBTask = _dal.StoreEventsAsync(realEvents);
            storeInDBTask.Wait();
            //group by our check ID, then update the checks
            var groupedEvents = realEvents.GroupBy(c => c.CheckID);
            var checks = (from @group in groupedEvents 
                          select @group.Select(t => t) 
                          into events 
                          select _checkRepository.ApplyEvents(events)).ToList();

			var allCommitted = checks.All(check => _checkRepository.Commit(check.ID).Result == CommitResult.SentToServer);
            if (allCommitted == false)
            {
                Logger.Log("Not all checks committed successfully!");
            }

            var upsertTask = _dal.UpsertEntitiesAsync(checks);
            upsertTask.Wait();

            Task.WaitAll(storeInDBTask, upsertTask);
        }

        public object Get(CheckRequest request)
        {
            try
            {
                var restaurant = GetRestaurantByFriendlyName(request.RestaurantFriendlyName);

                if (restaurant == null)
                {
                    throw new ArgumentException("No restaurant found for " + request.RestaurantFriendlyName);
                }
                IEnumerable<ICheck> res;
                if (request.CheckID.HasValue)
                {
                    var check = _checkRepository.GetByID(request.CheckID.Value);
                    if (check == null)
                    {
                        throw new ArgumentException("No check found for ID " + request.CheckID.Value);
                    }

                    res = new[] {check};
                }
                else
                {
					res = _checkRepository.GetAll().Cast<ICheck>().Where(c => c.RestaurantID == restaurant.ID);
                }

                if (request.PaymentStatus.HasValue)
                {
                    res = res.Where(r => r.PaymentStatus == request.PaymentStatus.Value);
                }

                return res;
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
                throw;
            }
            //return res;
        }
    }
}