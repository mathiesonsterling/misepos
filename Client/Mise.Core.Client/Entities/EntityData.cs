using System;

namespace Mise.Core.Client.Entities
{
    public class EntityData
    {
        public string Id{ get; set; }
        public byte[] Version{ get; set; }
        public bool Deleted{ get; set; }
        public DateTimeOffset? CreatedAt{get;set;}
        public DateTimeOffset? UpdatedAt{get;set;}
    }
}

