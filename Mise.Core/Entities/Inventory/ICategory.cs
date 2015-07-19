using System;
using Mise.Core.Entities.Base;

namespace Mise.Core.Entities.Inventory
{
	/// <summary>
	/// Basic defintion of a category
	/// </summary>
	public interface ICategory : IEntityBase, ITextSearchable
	{
		/// <summary>
		/// If set, this is the parent
		/// </summary>
		/// <value>The parent category I.</value>
		Guid? ParentCategoryID{get;}

		string Name{get;}

		/// <summary>
		/// If true, this is a default Mise category
		/// </summary>
		bool IsCustomCategory{get;}
	}
}

