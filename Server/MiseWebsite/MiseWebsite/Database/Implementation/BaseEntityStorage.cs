using System;
using Mise.Core.Entities.Base;

namespace MiseWebsite.Database.Implementation
{
    public abstract class BaseEntityStorage
    {
        protected BaseEntityStorage() { }

        protected BaseEntityStorage(IEntityBase source)
        {
            Id = source.Id;
            CreatedDate = source.CreatedDate;
            LastUpdatedDate = source.LastUpdatedDate;
            Revision = source.Revision.ToDatabaseString();
        }

        public Guid Id { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public string Revision { get; set; }
    }
}