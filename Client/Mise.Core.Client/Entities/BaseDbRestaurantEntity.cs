using System;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
namespace Mise.Core.Client.Entities
{
    public abstract class BaseDbRestaurantEntity<TEntityType, TConcrete> : BaseDbEntity<TEntityType, TConcrete>, IMiseRestaurantClientEntity
            where TEntityType : IEntityBase, IRestaurantEntityBase
            where TConcrete : EntityBase, TEntityType
    {
        public string RestaurantId { get; set; }

        protected BaseDbRestaurantEntity () : base ()
        {
        }

        protected BaseDbRestaurantEntity (TEntityType source) : base (source)
        {
            RestaurantId = source.RestaurantID.ToString ();
        }
    }
}

