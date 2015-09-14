using System;

using Mise.Core.Entities.Base;
using Mise.Core.ValueItems;

namespace Mise.Core
{
	public class EntityBase : IEntityBase
	{
		public virtual Guid Id{ get; set;}
		public DateTimeOffset CreatedDate{get;set;}
		public DateTimeOffset LastUpdatedDate{get;set;}
		public EventID Revision{ get; set; }

		public bool Equals(IEntityBase other)
		{
			return Id.Equals(other.Id) && Revision.Equals(other.Revision);
		}

        /// <summary>
        /// Helper method for cloning the fields in this entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newItem"></param>
        /// <returns></returns>
        protected virtual T CloneEntityBase<T>(T newItem) where T : EntityBase
        {
            newItem.CreatedDate = CreatedDate;
            newItem.Id = Id;
            newItem.LastUpdatedDate = LastUpdatedDate;
            newItem.Revision = Revision;

            return newItem;
        }
	}
}

