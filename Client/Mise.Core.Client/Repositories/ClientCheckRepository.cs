using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Events.Checks;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services.WebServices;
using Mise.Core.Common.Services;
using Mise.Core.Services;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Check;

namespace Mise.Core.Client.Repositories
{
	/// <summary>
	/// Check repository for use by terminals and clients
	/// </summary>
	public class ClientCheckRepository : BaseEventSourcedClientRepository<ICheck, ICheckEvent>, ICheckRepository
	{
		readonly IRestaurantTerminalService _service;
		public ClientCheckRepository (IRestaurantTerminalService service,
            IClientDAL dal, ILogger logger, IResendEventsWebService resend)
            : base(logger, dal, service, resend)
		{
		    _service = service;
		}


	    protected override Task<IEnumerable<ICheck>> LoadFromWebservice(Guid? restaurantID)
	    {
	        return _service.GetChecksAsync();
	    }

	    protected override async Task<IEnumerable<ICheck>> LoadFromDB(Guid? restaurantID)
	    {
            Logger.Log("Loading from DAL");
			var items = await DAL.GetEntitiesAsync<RestaurantCheck> ();
	        return items;
	    }

        IEnumerable<ICheck> GetOpenChecks()
        {
            return GetAll().Where(c => c.PaymentStatus != CheckPaymentStatus.Closed && c.PaymentStatus != CheckPaymentStatus.PaymentPending);
        }

        IEnumerable<ICheck> ICheckRepository.GetClosedChecks()
        {
            return GetAll().Where(c => c.PaymentStatus == CheckPaymentStatus.Closed || c.PaymentStatus == CheckPaymentStatus.PaymentPending);
        }

        public IEnumerable<ICheck> GetOpenChecks(IEmployee employee)
        {
            return employee == null ? GetOpenChecks() : GetOpenChecks().Where(c => c.LastTouchedServerID == employee.ID);
        }

        public IEnumerable<ICheck> GetChecksPriorToZ()
        {
            return GetAll().Where(c => c.PaymentStatus != CheckPaymentStatus.ClosedAndZed);
        }

	    protected override ICheck CreateNewEntity()
	    {
	        return new RestaurantCheck();
	    }

	    protected override bool IsEventACreation(IEntityEventBase ev)
        {
            return ev is CheckCreatedEvent;
        }

	    public override Guid GetEntityID(ICheckEvent ev)
	    {
	        return ev.CheckID;
	    }
	}
}

