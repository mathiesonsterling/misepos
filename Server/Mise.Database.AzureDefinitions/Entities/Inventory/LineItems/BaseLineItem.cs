using System.ComponentModel.DataAnnotations.Schema;

namespace Mise.Database.AzureDefinitions.Entities.Inventory.LineItems
{
    [ComplexType]
    public class BaseLineItem
    {
        public string DisplayName { get; set; }

        /// <summary>
        /// Reconciled name in our system, so we can combat vendor's differning
        /// </summary>
        public string MiseName { get; set; }

        /// <summary>
        /// UPC for this item, if we know it
        /// </summary>
        public string UPC { get; set; }

        /// <summary>
        /// If not null, this is how many units are in a case.  Allows us to convert cases to bottles
        /// </summary>
        /// <value>The size of the case.</value>
        public int? CaseSize { get; set; }

        public decimal Quantity { get; set; }
    }
}
