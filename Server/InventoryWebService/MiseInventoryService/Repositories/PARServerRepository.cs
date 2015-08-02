using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Server.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace MiseInventoryService.Repositories
{
    public class PARServerRepository : BaseAdminServiceRepository<IPar, IParEvent>, IParRepository
    {
        private readonly Dictionary<Guid, IPar> _oldVersions; 
        public PARServerRepository(ILogger logger, IEntityDAL entityDAL, IWebHostingEnvironment host) : base(logger, entityDAL, host)
        {
            _oldVersions = new Dictionary<Guid, IPar>();
        }

        public override IPar ApplyEvents(IEnumerable<IParEvent> events)
        {
            var evsList = events.ToList();
            var firstEv = evsList.First(ev => ev != null);
            _oldVersions[firstEv.ID] = Cache.GetByID(firstEv.ID);
            return base.ApplyEvents(evsList);
        }

        public Task<IPar> GetCurrentPAR(Guid restaurantID)
        {
            var items = GetAll().FirstOrDefault(p => p.RestaurantID == restaurantID && p.IsCurrent);
            return Task.FromResult(items);
        }


        protected override IPar CreateNewEntity()
        {
            return new Par(); 
        }

        public override Guid GetEntityID(IParEvent ev)
        {
            return ev.ParID;
        }

        protected override async Task LoadFromDB()
        {
            var pars = await EntityDAL.GetPARsAsync();
            Cache.UpdateCache(pars);
        }

        public override Task<CommitResult> Commit(Guid entityID)
        {
            return CommitBase(entityID, EntityDAL.UpdatePARAsync, EntityDAL.AddPARAsync);
        }
    }
}
