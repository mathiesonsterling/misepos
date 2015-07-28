using System;
using System.Linq;
using System.Threading.Tasks;

using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Services;
using Mise.Core.Services.WebServices;
using Mise.Core.Repositories;

namespace Mise.Core.Client.Repositories
{
	public class ClientParRepository : BaseEventSourcedClientRepository<IPAR, IPAREvent>, IPARRepository
	{
	    private readonly IPARWebService _webService;
        public ClientParRepository(ILogger logger, IClientDAL dal, IPARWebService webService, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
	    {
	        _webService = webService;
	    }

	    protected override IPAR CreateNewEntity()
	    {
	        return new PAR();
	    }

	    protected override bool IsEventACreation(IEntityEventBase ev)
	    {
	        return ev is PARCreatedEvent;
	    }

	    public override Guid GetEntityID(IPAREvent ev)
	    {
	        return ev.ParID;
	    }

	    public override async Task Load(Guid? restaurantID)
	    {
	        Loading = true;
			if (restaurantID.HasValue == false) {
				throw new ArgumentException ("Cannot load PARs until restaurant is set!");
			}

			var pars = await _webService.GetPARsForRestaurant (restaurantID.Value);

	        Cache.UpdateCache(pars);
	        Loading = false;
	    }

		public Task<IPAR> GetCurrentPAR (Guid restaurantID)
		{
			var par = GetAll ().FirstOrDefault (p => p.IsCurrent && p.RestaurantID == restaurantID);
			return Task.FromResult (par);
		}
	}
}

