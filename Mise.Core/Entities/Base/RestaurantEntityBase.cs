using System;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Base
{
	public abstract class RestaurantEntityBase : EntityBase, IRestaurantEntityBase
    {
        public Guid RestaurantID { get; set; }

        /// <summary>
        /// Helper method for cloning the fields in this entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
	    protected T CloneRestaurantBase<T>(T newItem) where T : RestaurantEntityBase
        {
            var fromBase = base.CloneEntityBase(newItem);
            fromBase.RestaurantID = RestaurantID;

            return fromBase;
        }
    }
}
