using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.Entities.Base;
using Mise.Core.Repositories;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceModelPortable.Responses;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceInterface
{
    /// <summary>
    /// Handles all events being pushed to the server
    /// </summary>
    public class EventsService : Service
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IReceivingOrderRepository _receivingOrderRepository;
        private readonly IApplicationInvitationRepository _appInviteRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPARRepository _parRepository;

        private readonly IEventStorageDAL _eventStorageDAL;

        private readonly EventDataTransportObjectFactory _eventFactory;
        private readonly ILogger _logger;

        private readonly List<EventDataTransportObject> _unprocessedEvents;
 
        public EventsService(IEventStorageDAL dal, IJSONSerializer serializer,
            IInventoryRepository inventoryRepository, IVendorRepository vendorRepository, 
            IPurchaseOrderRepository purchaseOrderRepository, IEmployeeRepository employeeRepository, 
            IPARRepository parRepository, IReceivingOrderRepository receivingOrderRepository, IApplicationInvitationRepository applicationInvitationRepository, 
            IRestaurantRepository restaurantRepository, IAccountRepository accountRepository, ILogger logger)
        {
            _eventStorageDAL = dal;
            _eventFactory = new EventDataTransportObjectFactory(serializer);
            _inventoryRepository = inventoryRepository;
            _vendorRepository = vendorRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _employeeRepository = employeeRepository;
            _receivingOrderRepository = receivingOrderRepository;
            _appInviteRepository = applicationInvitationRepository;
            _restaurantRepository = restaurantRepository;
            _accountRepository = accountRepository;
            _parRepository = parRepository;

            _logger = logger;

            _unprocessedEvents = new List<EventDataTransportObject>();
        }


        /// <summary>
        /// Main method for this service.  Gets events sent, stores them, then dispatches them to the 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<EventSubmissionResponse> Post(EventSubmission request)
        {
            if (request.Events == null || request.Events.Any() == false)
            {
                return new EventSubmissionResponse
                {
                    NumEventsProcessed = 0,
                    Result = false,
                    ErrorMessage = "no events were sent to server"
                };
            }

            var events = request.Events.ToList();
            bool storedEvents = false;
            try
            {
                storedEvents = await _eventStorageDAL.StoreEventsAsync(request.Events);
            }
            catch (Exception e)
            {
                _logger.HandleException(e);
                _unprocessedEvents.AddRange(events);
            }

            if (storedEvents == false)
            {
                _logger.Log("Unable to store events");
                _unprocessedEvents.AddRange(events);
            }

            var empRes =
                await HandleEventsIntoRepository(events, dto => _eventFactory.ToEmployeeEvent(dto), _employeeRepository);

            var vendorRes = await HandleEventsIntoRepository(events, dto => _eventFactory.ToVendorEvent(dto),
                _vendorRepository);

            var poRes = await HandleEventsIntoRepository(events, dto => _eventFactory.ToPurchaseOrderEvent(dto),
                _purchaseOrderRepository);

            var parRes = await HandleEventsIntoRepository(events, dto => _eventFactory.ToPAREvent(dto), _parRepository);

            var invRes = await HandleEventsIntoRepository(events, dto => _eventFactory.ToInventoryEvent(dto),
                _inventoryRepository);

            var receivingOrderRes = await HandleEventsIntoRepository(events,
                dto => _eventFactory.ToReceivingOrderEvent(dto), _receivingOrderRepository);

            var restRes = await HandleEventsIntoRepository(events, dto => _eventFactory.ToRestaurantEvent(dto),
                _restaurantRepository);

            var appInviteRes = await HandleEventsIntoRepository(events,
                dto => _eventFactory.ToApplicationInvitiationEvent(dto), _appInviteRepository);

            var accountsRes = await HandleEventsIntoRepository(events, dto => _eventFactory.ToAccountEvent(dto),
                _accountRepository);
            var tasks = new List<Tuple<string, bool>>
            {
                new Tuple<string, bool>("empEvents",empRes),
                new Tuple<string, bool>("vendor",vendorRes),
                new Tuple<string, bool>("purchaseOrder",poRes),
                new Tuple<string, bool>("par", parRes),
                new Tuple<string, bool>("Inventory",invRes),
                new Tuple<string, bool>("ReceivingOrder",receivingOrderRes),
                new Tuple<string, bool>("Restaurant", restRes),
                new Tuple<string, bool>("ApplicationInvitation", appInviteRes),
                new Tuple<string, bool>("Account", accountsRes)
            };


            if (tasks.All(t => t.Item2))
            {
                //sort which objects this should go to, for each repository
                return new EventSubmissionResponse
                {
                    Result = true,
                    NumEventsProcessed = request.Events.Count()
                };
            }

            var errors = tasks.Where(t => t.Item2 == false).Select(t => t.Item1);
            var errorString = string.Join(",", errors);
            _logger.Error("Errors in event processing :" + errorString);
            return new EventSubmissionResponse
            {
                Result = false,
                ErrorMessage = "something went wrong : " + errorString,
                NumEventsProcessed = -1
            };
        }

        private async Task<bool> HandleEventsIntoRepository<TEntity, TEventType>(
            IEnumerable<EventDataTransportObject> allEventDTOs,
            Func<EventDataTransportObject, TEventType> eventTransform,
            IEventSourcedEntityRepository<TEntity, TEventType> repository)
            where TEventType : class, IEntityEventBase
            where TEntity : IEventStoreEntityBase<TEventType> 
        {
            var events = allEventDTOs.Select(eventTransform.Invoke)
                .Where(ev => ev != null);

            //split the items up by type?

            var grouped = (from ev in events
                          group ev by repository.GetEntityID(ev)
                          into g
                          select new Tuple<Guid, IEnumerable<TEventType>>(g.Key, g.AsEnumerable())
                          ).ToList();

            if (grouped.Any() == false)
            {
                return true;
            }

            foreach (var entGroup in grouped)
            {
                try
                {
                    repository.StartTransaction(entGroup.Item1);
                    repository.ApplyEvents(entGroup.Item2);

                    await repository.CommitAll();
                }
                catch (Exception e)
                {
                    _logger.HandleException(e);
                    throw;
                }
            }

            return true;
        }

    }
}
