using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Common.Events.Inventory;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Inventory.Events;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Repositories;

namespace Mise.Core.Client.Repositories
{
	public class ClientParRepository : BaseEventSourcedClientRepository<IPar, IParEvent, Par>, IParRepository
	{
	    private readonly IParWebService _webService;
        public ClientParRepository(ILogger logger, IClientDAL dal, IParWebService webService, IResendEventsWebService resend)
            : base(logger, dal, webService, resend)
	    {
	        _webService = webService;
	    }

	    protected override IPar CreateNewEntity()
	    {
	        return new Par();
	    }


	    public override Guid GetEntityID(IParEvent ev)
	    {
	        return ev.ParID;
	    }

	    protected override Task<IEnumerable<Par>> LoadFromWebservice(Guid? restaurantID)
	    {
	        if (restaurantID.HasValue == false)
	        {
	            throw new ArgumentException("Cannot load PARS until restaurant is set");
	        }
	        return _webService.GetPARsForRestaurant(restaurantID.Value);
	    }

	    protected override async Task<IEnumerable<Par>> LoadFromDB(Guid? restaurantID)
	    {
	        var items = await DAL.GetEntitiesAsync<Par>();
	        return items;
	    }


		public Task<IPar> GetCurrentPAR (Guid restaurantID)
		{
			var par = GetAll ().FirstOrDefault (p => p.IsCurrent && p.RestaurantID == restaurantID);
			return Task.FromResult (par);
		}
	}
}

