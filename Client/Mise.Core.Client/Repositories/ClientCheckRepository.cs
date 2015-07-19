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
			IClientDAL dal, ILogger logger) : base(logger, dal, service)
		{
		    _service = service;
		}


		/// <summary>
		/// Loads our repository so it matches the current state of the server
		/// </summary>
		public override async Task Load(Guid? restaurantID)
		{
		    Loading = true;
			ICollection<ICheck> items = null;
		    var gotFromService = false;
				//load them into the DB as well
			Logger.Log ("Loading from service", LogLevel.Debug); 
		    try
		    {
		        items = (await _service.GetChecksAsync().ConfigureAwait(false)).ToList();
		        gotFromService = true;
		    }
		    catch (Exception e)
		    {
		        Logger.HandleException(e);
		    }

		    if(items == null)
		    {
		        items = (await LoadFromDB()).ToList();
		    }

		    if (gotFromService)
		    {
		        var upserttedSuccess = await DAL.UpsertEntitiesAsync(items).ConfigureAwait(false);
		        if (upserttedSuccess == false)
		        {
                    throw new Exception("Unable to update database with latest version!");
		        }
		    }

			Cache.UpdateCache (items);
		    IsFullyCommitted = true;
		    Loading = false;
		}

	    private async Task<IEnumerable<ICheck>> LoadFromDB()
	    {
            Logger.Log("Loading from DAL");
			return await DAL.GetEntitiesAsync<ICheck> ();
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

