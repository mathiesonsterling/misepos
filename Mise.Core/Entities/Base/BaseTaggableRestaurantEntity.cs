using System.Collections.Generic;
using Mise.Core.ValueItems;

namespace Mise.Core.Entities.Base
{
	public abstract class BaseTaggableRestaurantEntity : RestaurantEntityBase, ITaggable
	{
		/// <summary>
		/// Tags which come from Mise and should not be altered
		/// </summary>
		/// <value>The mise tags.</value>
		public List<Tag> MiseTags{get;set;}

		/// <summary>
		/// Tags specific to this restaurant
		/// </summary>
		/// <value>The user tags.</value>
		public List<Tag> RestaurantTags{get;set;}

		public IEnumerable<Tag> GetTags ()
		{
			var all = new List<Tag> (MiseTags);
			all.AddRange (RestaurantTags);
			return all;
		}

		public bool AddTag (Tag tag)
		{
			if(RestaurantTags.Contains (tag)){
				return false;
			}

			RestaurantTags.Add (tag);
			return true;
		}

		public bool RemoveTag (Tag tag)
		{
			if(RestaurantTags.Contains (tag)){
				return false;
			}

			return RestaurantTags.Remove (tag);
		}
	}
}

