using Mise.Core.ValueItems;
using Mise.Core.Entities.Base;
namespace Mise.Core.Entities.Menu
{
    public class MenuItemModifier : RestaurantEntityBase
    {
		public MenuItemModifier(){
			PriceChange = Money.None;
			PriceMultiplier = 1.0M;
		}

        public bool IsRequired
        {
            get;
            set;
        }
			

        public Money PriceChange
        {
            get;
            set;
        }

		public decimal PriceMultiplier{get;set;}

        public string Name
        {
            get;
            set;
        }
    }
}
