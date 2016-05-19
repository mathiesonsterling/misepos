using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Services.UtilityServices;
using Mise.Database.AzureDefinitions.Context;

namespace TransferMiseEntitesTool.Consumers
{
    class ApplicationInvitationConsumer : BaseConsumer<ApplicationInvitation, Mise.Database.AzureDefinitions.Entities.Accounts.ApplicationInvitation>
    {
        public ApplicationInvitationConsumer(IJSONSerializer jsonSerializer) : base(jsonSerializer)
        {
        }

        protected override async Task<Mise.Database.AzureDefinitions.Entities.Accounts.ApplicationInvitation> SaveEntity(StockboyMobileAppServiceContext db, ApplicationInvitation entity)
        {
            var rest = await db.Restaurants.FirstOrDefaultAsync(r => r.RestaurantID == entity.RestaurantID);
            var app = await db.MiseApplications.FirstOrDefaultAsync(a => a.AppTypeValue == (int) entity.Application);
            var destEmp = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.DestinationEmployeeID);
            var invEmp = await db.Employees.FirstOrDefaultAsync(e => e.EntityId == entity.InvitingEmployeeID);
            var dbEnt = new Mise.Database.AzureDefinitions.Entities.Accounts.ApplicationInvitation(entity, app, destEmp, invEmp, rest);

            db.ApplicationInvitations.Add(dbEnt);

            return dbEnt;
        }

        protected override Task<Mise.Database.AzureDefinitions.Entities.Accounts.ApplicationInvitation> GetSavedEntity(StockboyMobileAppServiceContext db, Guid id)
        {
            return db.ApplicationInvitations.FirstOrDefaultAsync(ai => ai.EntityId == id);
        }
    }
}
