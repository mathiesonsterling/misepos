using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Entities.People;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Repositories
{
	/// <summary>
	/// Check repository for use by terminals and clients
	/// </summary>
	public class ClientCheckRepository : BaseEventSourcedClientRepository<ICheck, ICheckEvent, RestaurantCheck>, ICheckRepository
	{
		readonly IRestaurantTerminalService _service;
		public ClientCheckRepository (IRestaurantTerminalService service,
            IClientDAL dal, ILogger logger, IResendEventsWebService resend)
            : base(logger, dal, service, resend)
		{
		    _service = service;
		}


	    protected override async Task<IEnumerable<RestaurantCheck>> LoadFromWebservice(Guid? restaurantID)
	    {
	        var items = await _service.GetChecksAsync();
	        return items.Cast<RestaurantCheck>();
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


	    public override Guid GetEntityID(ICheckEvent ev)
	    {
	        return ev.CheckID;
	    }
	}
}

