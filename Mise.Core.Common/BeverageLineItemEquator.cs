using Mise.Core.Entities.Inventory;

namespace Mise.Core.Common.Entities.Inventory
{
    /// <summary>
    /// Class that allows us to compare if two beverage line items refer to the same thing
    /// Example - a VendorLineItem of 750ml WildTurkey is equal to a RequisitionLineItem of 750ml WildTurkey, Quantity 3
    /// </summary>
    public static class BeverageLineItemEquator
    {
        public static bool AreSameBeverageLineItem(IBaseBeverageLineItem first, IBaseBeverageLineItem second)
        {
            if(string.IsNullOrEmpty(first.UPC) == false && string.IsNullOrEmpty(second.UPC) == false
                && first.UPC.Equals(second.UPC))
            {
                return true;
            }

            if (string.IsNullOrEmpty(first.MiseName) == false && string.IsNullOrEmpty(second.MiseName) == false
				&& first.MiseName.ToUpper().Equals(second.MiseName.ToUpper ())
                && first.Container != null && second.Container != null
                && first.Container.Equals(second.Container))
            {
                return true;
            }

			if(first.DisplayName.ToUpper () == second.DisplayName.ToUpper () && first.Container.Equals (second.Container)){
				return true;
			}

            return false;
        }

		public static bool IsItem(IBaseBeverageLineItem li, string name, string UPC){
			if(string.IsNullOrEmpty (UPC) == false && string.IsNullOrEmpty (li.UPC) == false){
				return UPC == li.UPC;
			}
			if(string.IsNullOrEmpty (li.DisplayName) == false  && string.IsNullOrEmpty (name) == false){
				if(li.DisplayName == name){
					return true;
				}

				return string.IsNullOrEmpty (li.MiseName) == false && li.MiseName == name;
			}

			return false;
		}
    }
}
