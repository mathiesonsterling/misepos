using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Repositories;
using Mise.Core.Common.Services;
using System;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities;

namespace Mise.Core.Client.Repositories
{
    public class ClientEmployeeRepository : BaseEventSourcedClientRepository<IEmployee, IEmployeeEvent>, IEmployeeRepository
    {
        readonly IInventoryEmployeeWebService _webService;
        readonly IClientDAL _clientDAL;


        public ClientEmployeeRepository(IInventoryEmployeeWebService webService, IClientDAL dal, ILogger logger, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
        {
            _webService = webService;
            _clientDAL = dal;
        }


        public override async Task Load(Guid? restaurantID)
        {
            Loading = true;
            IEnumerable<IEmployee> emps = null;
            Logger.Log("Loading employees from service", LogLevel.Debug);

            var needsDBLoad = true;
            try
            {
                emps = await _webService.GetEmployeesAsync();
                needsDBLoad = false;
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
            }

            if (needsDBLoad)
            {
                emps = await LoadFromDB();
            }

            var loadedEmps = emps.Where(e => e != null).ToList();
            await _clientDAL.UpsertEntitiesAsync(loadedEmps);
            Cache.UpdateCache(loadedEmps);
            Loading = false;
        }

        async Task<IEnumerable<IEmployee>> LoadFromDB()
        {
            Logger.Log("Could not get employees from web service, pulling from DAL", LogLevel.Debug);
            var items = await _clientDAL.GetEntitiesAsync<IEmployee>();
            return items.ToList();
        }

        public async Task<IEmployee> GetByEmailAndPassword(EmailAddress email, Password password)
        {
            //hash the password and compare
            /*var foundInCache = GetAll().FirstOrDefault(emp => 
                emp.Password.Equals(password)
                              && emp.GetEmailAddresses().Select(em => em.Value.ToUpper ()).Contains(email.Value.ToUpper ()));*/
            var foundInCache =
                GetAll().FirstOrDefault(emp => emp.Password.Equals(password) && emp.GetEmailAddresses().Contains(email));

            if (foundInCache != null)
            {
                return foundInCache;
            }

            var wsEmp = await _webService.GetEmployeeByPrimaryEmailAndPassword(email, password);
            if (wsEmp != null && wsEmp.ID != Guid.Empty)
            {
                Cache.UpdateCache(wsEmp, ItemCacheStatus.Clean);
                return wsEmp;
            }
            return null;
        }

        public IEmployee GetByPasscode(string passcode)
        {
            return Cache.GetAll().FirstOrDefault(e => e.Passcode == passcode);
        }

        protected override IEmployee CreateNewEntity()
        {
            return new Employee();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is EmployeeCreatedEvent;
        }

        public override Guid GetEntityID(IEmployeeEvent ev)
        {
            return ev.EmployeeID;
        }

        /// <summary>
        /// Need to accept bad logins and other items for things not in the repository
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        public override IEmployee ApplyEvents(IEnumerable<IEmployeeEvent> events)
        {
            var oEvents = events.OrderBy(e => e.CreatedDate).ThenBy(e => e.EventOrderingID);
            var employeeID = oEvents.FirstOrDefault().EmployeeID;

            if (oEvents.Any() && oEvents.FirstOrDefault() != null)
            {
                if (UnderTransaction.ContainsKey(employeeID) == false)
                {
                    StartTransaction(employeeID);
                }
            }

            var bundle = UnderTransaction[employeeID];
            foreach (var employeeEvent in oEvents)
            {
                if (bundle.NewVersion == null)
                {
                    bundle.NewVersion = GetByID(employeeEvent.EmployeeID);
                    //if our event isn't a creation, we might have a problem
                    if (bundle.NewVersion == null)
                    {
                        if (employeeEvent is BadLoginAttemptEvent)
                        {
                            return null;
                        }
                        if (IsEventACreation(employeeEvent))
                        {
                            bundle.NewVersion = CreateNewEntity();
                        }
                        else
                        {
                            throw new ArgumentException("Event specified for employee " + employeeEvent.EmployeeID + " that can't be found");
                        }
                    }
                }
                bundle.NewVersion.When(employeeEvent);
                Logger.Log("Applied event " + employeeEvent.GetType(), LogLevel.Debug);
            }

            //events go to the basic service to get added here!
            try
            {
                //add them to our staging cache
                if (bundle != null)
                {
                    bundle.Events.AddRange(oEvents);
                }
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
            }

            return bundle == null ? null : bundle.NewVersion;
        }
    }
}
