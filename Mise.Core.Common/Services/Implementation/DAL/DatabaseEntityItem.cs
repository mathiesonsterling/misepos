using System;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.Implementation.DAL
{
	/// <summary>
	/// Utility class allowing us to store entites
	/// </summary>
	public class DatabaseEntityItem
	{
		public Guid ID{ get; set;}
		public ItemCacheStatus Status{get;set;}
		public EventID RevisionNumber{get;set;}
		public Type Type{get;set;}
		public string JSON{ get; set;}
	}
}

