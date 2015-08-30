using System;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;

namespace MiseReporting.Models
{
    public class ParViewModel
    {
        public DateTime? DateCreated { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string DoneByEmployee { get; set; }

        public Guid Id { get; set; }

        public int NumLineItems { get; set; }

        public ParViewModel()
        {
            
        }

        public ParViewModel(IPar source, IEmployee emp)
        {
            if (source == null)
            {
                throw new ArgumentException("Source cannot be null for Par");
            }
            LastUpdated = source.LastUpdatedDate.LocalDateTime;
            DateCreated = source.CreatedDate.LocalDateTime;
            Id = source.ID;

            if (emp != null)
            {
                DoneByEmployee = emp.DisplayName;
            }

            NumLineItems = source.GetBeverageLineItems().Count();
        }
    }
}
