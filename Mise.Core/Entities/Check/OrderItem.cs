using System;
using System.Collections.Generic;
using System.Linq;
using Mise.Core.Entities.Menu;
using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Check
{
	public class OrderItem : RestaurantEntityBase
    {

        public OrderItem()
        {
			_modifiers = new List<MenuItemModifier>();
            _orderDestinations = new List<OrderDestination>();
            TimePlaced = DateTime.Now;
			Status = OrderItemStatus.Ordering;
        }

        public MenuItem MenuItem
        {
            get;
            set;
        }

		IList<MenuItemModifier> _modifiers;
        public IEnumerable<MenuItemModifier> Modifiers {
			get{ return _modifiers;}
			set{ _modifiers = value.ToList ();}
        }

		public void AddModifier(MenuItemModifier mod){
			_modifiers.Add (mod);
		}

		public void RemoveModifier(MenuItemModifier mod){
			_modifiers.Remove (mod);
		}

        public OrderItemStatus Status
        {
            get;
            set;
        }
		

        public string Name { get { return MenuItem.Name; } }


        public Money Total {
			get { 
				return Modifiers.Any () ? GetModifiedTotal (MenuItem, _modifiers) : MenuItem.Price;
			}
        }

		public static Money GetModifiedTotal(MenuItem menuItem, IList<MenuItemModifier> mods){
			if (mods.Any ()) {
				var topMult = mods.OrderByDescending (o => o.PriceMultiplier).First ();
				var price = menuItem.Price;
				price = price.Multiply (topMult.PriceMultiplier);

				return mods.Aggregate (price, (current, mod) => current.Add (mod.PriceChange));
			}
			return menuItem.Price;
		}

		public bool NeedsModifiers{ 
			get { 
				//if we have any required modified on the menu item, and we don't have an instance of them
				return MenuItem.PossibleModifiers
					.Where (m => m.Required)
					.Any (m => _modifiers.Select (oiM => oiM.ID).Contains (m.ID) == false);
			} 
		}

        public void AddModifiers(IList<MenuItemModifier> mods)
        {
            foreach (var m in mods)
            {
				AddModifier (m);
            }
        }

        public void ClearModifiers()
        {
            Modifiers = new List<MenuItemModifier>();
        }

		public string Memo { get; set; }

        public OrderItem Clone()
        {
            var newOI = new OrderItem
                            {
				CreatedDate = CreatedDate, 
				ID = ID, 
				MenuItem = MenuItem, 
				Status = Status,
				EmployeeWhoComped =  EmployeeWhoComped,
				IsOrdering = IsOrdering,
				LastUpdatedDate = LastUpdatedDate,
				PlacedByID = PlacedByID,
				RestaurantID = RestaurantID,
				Revision = Revision,
				TimePlaced = TimePlaced,
                Memo = Memo
			};
            //copy the modifiers
            foreach (var mi in Modifiers)
            {
                newOI.AddModifier( mi);
            }

            //copy destinations
            foreach (var dest in Destinations)
            {
                newOI.AddDestination(dest);
            }
            return newOI;
        }

        public DateTime TimePlaced { get; set; }

        public Guid PlacedByID { get; set; }

	    private readonly List<OrderDestination> _orderDestinations;
		public IEnumerable<OrderDestination> Destinations {
            get { return _orderDestinations; }
		}

	    public void AddDestination(OrderDestination dest)
	    {
	        _orderDestinations.Add(dest);
	    }


		public bool IsOrdering {
			get;
			set;
		}

		public bool IsComped {
			get{ return EmployeeWhoComped.HasValue;}
		}

		/// <summary>
		/// If a value, this is the employee who pushed the order item
		/// </summary>
		/// <value>The employee who comped.</value>
		public Guid? EmployeeWhoComped{ get; set;}
    }
}
