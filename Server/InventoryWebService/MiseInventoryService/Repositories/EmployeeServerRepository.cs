using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Employee;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.InventoryWebService.ServiceInterface.Exceptions;

namespace MiseInventoryService.Repositories
{
    public class EmployeeServerRepository : BaseAdminServiceRepository<IEmployee, IEmployeeEvent>, IEmployeeRepository
    {
        public EmployeeServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) 
            : base(logger, entityDAL, host)
        {
        }

        public override IEmployee ApplyEvents(IEnumerable<IEmployeeEvent> events)
        {
            ICollection<IEmployeeEvent> allEvents = events.ToList();

            var createEvent = allEvents.FirstOrDefault(ev => ev.EventType == MiseEventTypes.EmployeeCreatedEvent);
            if (createEvent != null)
            {
                CheckValidEmployeeCreation(createEvent as EmployeeCreatedEvent);
            }

            return base.ApplyEvents(allEvents);
        }

        public override IEmployee ApplyEvent(IEmployeeEvent entEvent)
        {
            return ApplyEvents(new[] {entEvent});
        }

        /// <summary>
        /// Inspects our creation event, and 
        /// </summary>
        /// <param name="createEvent"></param>
        /// <returns></returns>
        private void CheckValidEmployeeCreation(EmployeeCreatedEvent createEvent)
        {
            if (createEvent != null)
            {
                //check if our email is already used
                var email = createEvent.Email;

                var othersWithEmail = EmailIsTaken(email);

                if (othersWithEmail)
                {
                    throw new EmailAlreadyInUseException(email);
                }
            }
        }

        private bool EmailIsTaken(EmailAddress email)
        {
            if (email == null)
            {
                return false;
            }
            return GetAll()
                    .SelectMany(e => e.GetEmailAddresses())
                    .Any(e => e != null && e.Equals(email));
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdateEmployeeAsync, EntityDAL.AddEmployeeAsync);
        }

        protected override IEmployee CreateNewEntity()
        {
            return new Employee();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is EmployeeRegisteredForInventoryAppEvent || ev is EmployeeCreatedEvent;
        }

        public override Guid GetEntityID(IEmployeeEvent ev)
        {
            return ev.EmployeeID;
        }

        protected override async Task LoadFromDB()
        {
            var emps = await EntityDAL.GetInventoryAppUsingEmployeesAsync();

            Cache.UpdateCache(emps);
        }

        public IEmployee GetByPasscode(string passcode)
        {
            throw new InvalidOperationException("No passcodes valid on server!");
        }

        public Task<IEmployee> GetByEmailAndPassword(EmailAddress email, Password password)
        {
            var possibles = GetAll().Where(e => e.Password != null && e.Password.Equals(password));

            //var emp = possibles.FirstOrDefault(e => e.GetEmailAddresses().Select(em => em.Value.ToUpper()).Contains(email.Value.ToUpper()));
            var emp = possibles.FirstOrDefault(e => e.GetEmailAddresses().Contains(email));
            return Task.FromResult(emp);
        }
    }
}
