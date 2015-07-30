using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Check;
using Mise.Core.Entities.Check.Events;
using Mise.Core.Repositories;
using Mise.Core.Server.Services;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities;
using Mise.Core.Common.Repositories.Base;
namespace Mise.Core.Server.Repositories
{
	public class CheckRestaurantServerRepository : BaseEventSourcedRepository<ICheck, ICheckEvent>, ICheckRepository
	{
        readonly IRestaurantServerDAL _restaurantServerDAL;
        readonly IMiseAdminServer _server;
        private bool _loadedFromServer;
        public Guid? RestaurantID { get; private set; }
        public CheckRestaurantServerRepository(IMiseAdminServer server, IRestaurantServerDAL dal, ILogger logger, Guid? restaurantID)
            : base(logger)
        {
            _server = server;
            _restaurantServerDAL = dal;
            RestaurantID = restaurantID;
        }

		#region implemented abstract members of BaseEventSourcedRepository


		protected override ICheck CreateNewEntity ()
		{
			throw new NotImplementedException ();
		}


		protected override bool IsEventACreation (IEntityEventBase ev)
		{
			throw new NotImplementedException ();
		}

	    public override Guid GetEntityID(ICheckEvent ev)
	    {
	        return ev.CheckID;
	    }

	    #endregion


		#region implemented abstract members of CheckRepository


		public IEnumerable<ICheck> GetOpenChecks (Mise.Core.Entities.People.IEmployee employee)
		{
			throw new NotImplementedException ();
		}


		public IEnumerable<ICheck> GetClosedChecks ()
		{
			throw new NotImplementedException ();
		}


		public IEnumerable<ICheck> GetChecksPriorToZ ()
		{
			throw new NotImplementedException ();
		}


		#endregion

	    public bool Loading { get; private set; }

	    public Task Load(Guid? restaurantID)
        {
			return Task.Run (() => {
				List<ICheck> items = null;
				try {
					items = restaurantID.HasValue
                    ? _server.GetCheckSnapshots (restaurantID.Value).ToList ()
                    : _server.GetAllChecks ().ToList ();
					_loadedFromServer = true;
					//load them into the DB as well
					Logger.Log ("Loading from service", LogLevel.Debug);
					IEnumerable<ICheck> items1 = items;
					var loadTask = _restaurantServerDAL.UpsertEntitiesAsync (items1);

					Task.WaitAll (new Task[] { loadTask });
				} catch (Exception e) {
					Logger.HandleException (e, LogLevel.Debug);
					//if we didn't get items from the service, we need to load them from the DB
					if (_loadedFromServer == false) {
						Logger.Log ("Can't get service, loading from DAL", LogLevel.Debug);
						items = _restaurantServerDAL.GetEntitiesAsync<RestaurantCheck> (restaurantID).Result.ToList<ICheck> ();
					}
					//TODO we should keep trying to get to the server
				}
				Cache.UpdateCache (items);
				IsFullyCommitted = true;
			});
        }

		public override Task<CommitResult> Commit(Guid entityID)
        {
			throw new NotImplementedException ();
        }

	    public Task<CommitResult> CommitOnlyImmediately(Guid entityID)
	    {
	        return Commit(entityID);
	    }
	}
}

