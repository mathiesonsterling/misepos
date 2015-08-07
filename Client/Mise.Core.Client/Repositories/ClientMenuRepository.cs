using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Common.Services;
using Mise.Core.Common.Services.WebServices;
using Mise.Core.Entities.Menu;
using Mise.Core.Repositories;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Client.Repositories
{
	public class ClientMenuRepository : BaseRepository<Menu>, IMenuRepository
	{
		private readonly IRestaurantTerminalService _webService;
		private readonly IClientDAL _clientDAL;
		private readonly ILogger _logger;
		public ClientMenuRepository (IRestaurantTerminalService service, 
			IClientDAL dal, ILogger logger)
		{
			_webService = service;
			_clientDAL = dal;
			_logger = logger;
		}

		public void Load(){
			_logger.Log ("Loading menus from web service", LogLevel.Debug);
			Menu currMenu;
			try{
			    var menuTask = _webService.GetMenusAsync()
			        .ContinueWith(task =>
			        {
						if(task.IsFaulted){
							if(task.Exception != null){
								foreach(var t in task.Exception.Flatten ().InnerExceptions){
									_logger.HandleException (t);
								}
							}
							_logger.Log("Error retrieving menu from service");

							return _clientDAL.GetEntitiesAsync<Menu>().Result;
						}
			            var res = task.Result;
			            if(res == null){
			                _logger.Log("Error, recieved null value from webservice.  Will attempt to load from DB");
			                throw new Exception("Menu recieved from web service is null!");
			            }
			            _logger.Log("Storing menu in database", LogLevel.Debug);
			            try{
			                var upsert = _clientDAL.UpsertEntitiesAsync(res);
			                upsert.Wait();
			            } catch(Exception e){
			                _logger.Log("Error storing menu in the database");
			                _logger.HandleException(e);
			                throw;
			            }
			            return res;
			        });

				currMenu = menuTask.Result.FirstOrDefault () as Menu;
			}
			catch(Exception e){
                _logger.HandleException(e);
				try{
					currMenu = _clientDAL.GetEntitiesAsync<Menu>().Result.FirstOrDefault();
				} catch(Exception ex){
					_logger.Log ("Error retrieving menu from the database");
					_logger.HandleException(ex);
					throw;
				}
			}
			Cache.UpdateCache (currMenu, ItemCacheStatus.Clean);
		}

		#region IMenuRepository implementation

		public Menu GetCurrentMenu ()
		{
			return Cache.GetAll ().FirstOrDefault ();
		}

		#endregion

		public override Task<CommitResult> Commit (Guid entityID)
		{
			throw new InvalidOperationException("Cannot commit menus on client repository!");
		}
			
	}
}

