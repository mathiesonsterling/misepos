using System;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common;
using Mise.Core.Common.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class EmployeeServerRepository : BaseAdminServiceRepository<IEmployee, IEmployeeEvent>, IEmployeeRepository
    {
        public EmployeeServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
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
            return ev is EmployeeRegisteredForInventoryAppEvent;
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
