using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Entities;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Repositories;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.People;
using Mise.Core.Entities.People.Events;
using Mise.Core.Server.Services;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;
using Mise.Core.Common.Entities;
namespace Mise.Core.Server.Repositories
{
	public class EmployeeRestaurantServerRepository : BaseEventSourcedRepository<IEmployee, IEmployeeEvent>, IEmployeeRepository
    {
        public Guid? RestaurantID { get; private set; }
        private readonly IMiseAdminServer _server;
        private readonly IRestaurantServerDAL _dal;
		public EmployeeRestaurantServerRepository(IMiseAdminServer server, IRestaurantServerDAL dal, ILogger logger, Guid? restaurantID) : base(logger)
        {
            _server = server;
            _dal = dal;
            RestaurantID = restaurantID;
        }


        #region implemented abstract members of BaseRepository

		#region implemented abstract members of BaseEventSourcedRepository

		protected override IEmployee CreateNewEntity ()
		{
			return new Employee ();
		}

		protected override bool IsEventACreation (IEntityEventBase ev)
		{
			throw new NotImplementedException ();
		}

	    public override Guid GetEntityID(IEmployeeEvent ev)
	    {
	        return ev.EmployeeID;
	    }

	    #endregion

		#region implemented abstract members of EmployeeRepository

		public IEmployee GetByPasscode (string passcode)
		{
			throw new NotImplementedException ();
		}

		public Task<IEmployee> GetByEmailAndPassword(EmailAddress email, Password password)
		{
		    var emp =
		        GetAll()
		            .FirstOrDefault(e => e.GetEmailAddresses().Contains(email) && e.Password.HashValue == password.HashValue);

		    return Task.FromResult(emp);
		}

	    public Task<IEmployee> RegisterNewEmployee(EmailAddress email, Password password, IRestaurant restaurant)
	    {
	        throw new NotImplementedException();
	    }

	    #endregion

        private bool _loadedFromServer;


        public void Load()
        {
			Load(RestaurantID).Wait();
        }

	    public bool Loading { get; private set; }

	    public Task Load(Guid? restaurantId)
        {
			return Task.Run (() => {
				IList<IEmployee> emps = null;
				try {
					Logger.Log ("Loading employees from service", LogLevel.Debug);
					if (restaurantId.HasValue) {
						emps = _server.GetEmployeeSnapshots (restaurantId.Value).Where (e => e != null).ToList ();
					} else {
						emps = _server.GetAllEmployees ().ToList ();
					}
					_loadedFromServer = true;

					var emps1 = emps;
					var upsertTask = Task.Factory.StartNew (() => {
						Logger.Log ("Updating entites from webservice", LogLevel.Debug);
						var ut = _dal.UpsertEntitiesAsync (emps1);
						ut.Wait ();
					});

					Task.WaitAll (new[] { upsertTask });
				} catch (Exception e) {
					Logger.HandleException (e, LogLevel.Debug);

					if (_loadedFromServer == false) {
						Logger.Log ("Could not get employees from web service, pulling from DAL", LogLevel.Debug);
						emps = _dal.GetEntitiesAsync<Employee> ().Result.ToList<IEmployee> ();
					}
				}
				Cache.UpdateCache (emps);
				IsFullyCommitted = _loadedFromServer;
			});
        }


		public override Task<CommitResult> Commit(Guid employeeID)
        {
			throw new NotImplementedException ();
            //commit the events we have here
            /*if (UnderTransaction.ContainsKey(employeeID) == false)
            {
                //done
                return CommitResult.Error;
            }
            
            var theseEvents = UnderTransaction[employeeID].Events;
            //close the repos, sending our events over async
            var eventTask = Task<bool>.Factory.StartNew(() =>
            {
                Logger.Log("Storing and sending events for " + employeeID, LogLevel.Debug);

                try
                {
                    return _dal.StoreEventsAsync(theseEvents).Result;
                }
                catch (Exception e)
                {
                    Logger.HandleException(e);
                    return false;
                }
            })
            .ContinueWith(task => _dal.UpdateEventStatusesAsync(theseEvents, ItemCacheStatus.BeginSendToAdminServer).Result)
            .ContinueWith(task =>
            {
                if (task.IsFaulted || task.Result == false)
                {
                    Logger.Log("Unable to set event status!");
                    return false;
                }

                //SEND EVENTS TO THE RESTAURANT SER
                var restId = theseEvents.First().RestaurantID;
                var res = _server.SendEvents(restId, theseEvents);


                //once we get our results, we mark the events as sent
                return res;
            }).ContinueWith(task =>
            {
                if (task.IsFaulted || task.Result == false)
                {
                    Logger.Log("Unable to store and send events!");
                    return false;
                }
                return _dal.UpdateEventStatusesAsync(theseEvents, ItemCacheStatus.SentToAdminServer).Result;
            })
            .ContinueWith(task =>
            {
                if (task.IsFaulted || task.Result == false)
                {
                    Logger.Log("Count not update status to Sent!");
                }
                else
                {
                    UnderTransaction.Remove(employeeID);
                }
            });


            var entTask = Task<IEnumerable<IEntityBase>>.Factory.StartNew(() =>
            {
                Logger.Log("Updating entities for " + employeeID, LogLevel.Debug);
                var emp = GetByID(employeeID);
                if (emp == null)
                {
                    throw new Exception("Unable to retrieve employee with ID " + employeeID);
                }
                var ents = new List<IRestaurantEntityBase> {emp};
                if (_dal.UpsertEntitiesAsync(ents).Result)
                {
                    return ents;
                }
                throw new Exception("Unable to upsert our entity");
            })
            .ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    foreach (var ex in task.Exception.Flatten().InnerExceptions)
                    {
                        Logger.HandleException(ex);
                    }
                }
                Logger.Log("Updating cache for " + employeeID, LogLevel.Debug);
                var res = task.Result.ToList();
                if (res == null || res.Any() == false)
                {
                    Logger.Log("Unable to store check in DB!");
                }
                else
                {
                    foreach (var dbEmp in res.OfType<IEmployee>())
                    {
                        Cache.UpdateCache(dbEmp, ItemCacheStatus.TerminalDB);
                    }
                }
            });

            Task.WaitAll(eventTask, entTask);
            return true;*/
        }

	    public Task<CommitResult> CommitOnlyImmediately(Guid entityID)
	    {
	        return Commit(entityID);
	    }

	    #endregion
    }
}
