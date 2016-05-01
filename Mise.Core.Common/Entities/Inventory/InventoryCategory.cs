using System;
using System.Collections.Generic;
using Mise.Core.ValueItems.Inventory;
using Mise.Core.Entities;
using Mise.Core.Entities.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
	/// <summary>
	/// Category for inventory items
	/// </summary>
	public class InventoryCategory : EntityBase, IInventoryCategory
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

	    public bool IsAssignable { get; set; }

	    public bool ContainsSearchString (string searchString)
		{
		    return (false == string.IsNullOrEmpty(Name)) && Name.ToUpper().Contains (searchString.ToUpper());
		}

        public List<LiquidContainer> PreferredContainers{ get; set; }
        public IEnumerable<LiquidContainer> GetPreferredContainers(){
            return PreferredContainers;
        }
	}
}

