using System;
using System.Collections.Generic;
using System.Collections;

using Mise.Core.ValueItems;
namespace Mise.Core
{
	/// <summary>
	/// Represents any item which can be tagged.  
	/// </summary>
	public interface ITaggable
	{
		/// <summary>
		/// Retrive all tags this item has
		/// </summary>
		/// <returns>The tags.</returns>
		IEnumerable<Tag> GetTags ();

		/// <summary>
		/// Adds a tag, if it doesn't already exist
		/// </summary>
		/// <param name="tag">Tag.</param>
		bool AddTag(Tag tag);

		/// <summary>
		/// Remove a tag, if allowed.  Some tags might not be modifiable!
		/// </summary>
		/// <returns><c>true</c>, if tag was removed, <c>false</c> otherwise.</returns>
		/// <param name="tag">Tag.</param>
		bool RemoveTag (Tag tag);
	}
}

