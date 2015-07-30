using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mise.Core.Common.Repositories.Base;
using Mise.Core.Common.Services.DAL;
using Mise.Core.Entities.Base;
using Mise.Core.Services;
using Mise.Core.Services.UtilityServices;
using Mise.Core.ValueItems;

namespace Mise.Core.Server.Repositories
{
	/// <summary>
	/// Basic repository to handle getting event, combining them, and sending them to the main service
	/// Not used for checks 
	/// </summary>
	public class BaseRestaurantServerRepository<TEntity> : BaseRepository<TEntity>
		where TEntity:class, IRestaurantEntityBase
	{

		readonly IRestaurantServerDAL _dal;

        public bool Loading { get; protected set; }

        public Guid? RestaurantID { get; private set; }
		protected ILogger Logger{ get; private set;}
		protected BaseRestaurantServerRepository (IRestaurantServerDAL dal, ILogger logger, Guid? restaurantID)
		{
			_dal = dal;
		    RestaurantID = restaurantID;
			Logger = logger;
		}
			
		public override Task<CommitResult> Commit (Guid entityID)
		{
			throw new NotImplementedException ();
		}

        public Task<bool> CommitAll()
        {
            throw new NotImplementedException();
        }

	    public override TEntity GetByID (Guid id)
		{
		    var item = _dal.GetEntityByIDAsync<TEntity>(id).Result;

		    Cache.UpdateCache(item, ItemCacheStatus.Clean);
		    return item;
		}

		public override IEnumerable<TEntity> GetAll () 
		{
			throw new NotImplementedException();
		}

		public async Task<ICollection<TEntity>> CreateAsync(ICollection<TEntity> entities)
		{
		    await _dal.UpsertEntitiesAsync(entities);
		    return entities;
		}

		public async Task<ICollection<TEntity>> UpdateAsync(ICollection<TEntity> entities){
			return await CreateAsync (entities);
		}
	}
}

