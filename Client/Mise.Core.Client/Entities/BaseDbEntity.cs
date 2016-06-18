using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Client.ValueItems;
namespace Mise.Core.Client.Entities
{
    public abstract class BaseDbEntity<TEntityType, TConcrete> : EntityData, IMiseClientEntity
        where TEntityType : IEntityBase
        where TConcrete : EntityBase, TEntityType
    {
        protected BaseDbEntity() 
        { 
            Revision = new EventIDDb();
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        protected BaseDbEntity(TEntityType source)
        {
            var idVal = source.Id != Guid.Empty ? source.Id : Guid.NewGuid();
            Id = idVal.ToString();
            EntityId = idVal;
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

        protected static string IDListToString(IEnumerable<Guid> ids)
        {
            return string.Join(",", ids.Select(i => i.ToString()));
        }

        protected static IList<Guid> StringToIDList(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return new List<Guid>();
            }
            var ids = source.Split(',');
            return ids.Select(i => i.Trim()).Select(Guid.Parse).ToList();
        }
    }
}
