using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Database.AzureDefinitions.ValueItems;

namespace Mise.Database.AzureDefinitions.Entities
{
    public abstract class BaseDbEntity<TEntityType, TConcrete> : EntityData 
        where TEntityType : IEntityBase
        where TConcrete : EntityBase, TEntityType
    {
        protected BaseDbEntity() 
        { 
            Revision = new EventIDDb();
        }

        protected BaseDbEntity(TEntityType source)
        {
            EntityId = source.Id;
            CreatedAt = source.CreatedDate;
            UpdatedAt = source.LastUpdatedDate;
            Revision = new EventIDDb(source.Revision);
        }

        /// <summary>
        /// Lets the subitem tell us how to translate the DB item to the Business entity
        /// </summary>
        /// <returns></returns>
        protected abstract TConcrete CreateConcreteSubclass();

        public TConcrete ToBusinessEntity()
        {
            var entity = CreateConcreteSubclass();
            entity.Id = EntityId;
            entity.CreatedDate = CreatedAt ?? DateTimeOffset.MinValue;
            entity.LastUpdatedDate = UpdatedAt??DateTimeOffset.MinValue;
            entity.Revision = Revision;

            return entity;
        } 

        public Guid EntityId { get; set; }

        public EventIDDb Revision { get; set; }

    }
}
