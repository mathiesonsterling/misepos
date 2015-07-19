using System;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.Implementation.DAL
{
	public class DatabaseEventItem
	{
		public Guid ID{ get; set;}
		public ItemCacheStatus Status{get;set;}
		public long RevisionNumber{get;set;}
		public Type Type{get;set;}
		public string JSON{ get; set;}

        public DateTimeOffset CreatedOn { get; set; }
	}
}

