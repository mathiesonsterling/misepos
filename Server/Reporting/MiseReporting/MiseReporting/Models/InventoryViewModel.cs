using System;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;

namespace MiseReporting.Models
{
    public class InventoryViewModel
    {
        public DateTime? DateCompleted { get; set; }

        public DateTime DateCreatedUTC { get; set; }

        public DateTime DateCreated { get; set; }

        public string DoneByEmployee { get; set; }
        public Guid Id { get; set; }

        public int NumLineItems { get; set; }

        public InventoryViewModel() { }

        public InventoryViewModel(IInventory source, IEmployee emp)
        {
            DateCompleted = source.DateCompleted?.LocalDateTime;

            if (emp != null)
            {
                DoneByEmployee = emp.DisplayName;
            }

            DateCreated = source.CreatedDate.ToLocalTime().LocalDateTime;
            DateCreatedUTC = source.CreatedDate.DateTime;

            Id = source.Id;
            NumLineItems = source.GetBeverageLineItems().Count();
        }
    }
}
