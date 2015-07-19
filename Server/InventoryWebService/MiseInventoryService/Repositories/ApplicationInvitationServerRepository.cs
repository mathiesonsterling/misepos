using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.ApplicationInvitations;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{ 
    public class ApplicationInvitationServerRepository : BaseAdminServiceRepository<IApplicationInvitation, IApplicationInvitationEvent>, IApplicationInvitationRepository
    {
        public ApplicationInvitationServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdateApplicationInvitation, EntityDAL.AddApplicationInvitiation);
        }

        protected override IApplicationInvitation CreateNewEntity()
        {
            return new ApplicationInvitation();
        }

        protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is EmployeeInvitedToApplicationEvent;
        }

        public override Guid GetEntityID(IApplicationInvitationEvent ev)
        {
            return ev.InvitationID;
        }

        protected override async Task LoadFromDB()
        {
            try
            {
                var res = await EntityDAL.GetApplicationInvitations();
                Cache.UpdateCache(res);
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
            }
        }

        public Task<IEnumerable<IApplicationInvitation>> GetOpenInvitesForEmail(EmailAddress address)
        {
            var items = GetAll().Where(ai => ai.Status == InvitationStatus.Created || ai.Status == InvitationStatus.Sent)
                .Where(ai => ai.DestinationEmail.Equals(address));

            return Task.FromResult(items);
        }

        public Task<IEnumerable<IApplicationInvitation>> GetInvitesForRestaurant(Guid restaurantID)
        {
            var items = GetAll().Where(ai => ai.RestaurantID == restaurantID);
            return Task.FromResult(items);
        }

        public async Task Load(EmailAddress email)
        {
            try
            {
                //TODO change this to load one on the DB
                var items = await EntityDAL.GetApplicationInvitations();
                var rightItems = items.Where(ai => ai.DestinationEmail.Equals(email));
                Cache.UpdateCache(rightItems);
            }
            catch (Exception e)
            {
                Logger.HandleException(e);
            }
        }
    }
}
