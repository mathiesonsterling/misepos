using System;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	/// <summary>
	/// Category for inventory items
	/// </summary>
	public class ItemCategory : EntityBase, ICategory
	{
		public Guid? ParentCategoryID {
			get;
			set;
		}
		public string Name {
			get;
			set;
		}
		public bool IsCustomCategory {
			get;
			set;
		}

		public bool ContainsSearchString (string searchString)
		{
		    return (false == string.IsNullOrEmpty(Name)) && Name.ToUpper().Contains (searchString.ToUpper());
		}
	}
}

