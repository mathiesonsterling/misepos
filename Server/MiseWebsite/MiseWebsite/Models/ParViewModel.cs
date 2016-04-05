using System;
using System.ComponentModel;
using System.Linq;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;

namespace MiseWebsite.Models
{
    public class ParViewModel
    {
        [DisplayName("Date Created")]
        public DateTime? DateCreated { get; set; }

        [DisplayName("Last Updated")]
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
            Id = source.Id;

            if (emp != null)
            {
                DoneByEmployee = emp.DisplayName;
            }

            NumLineItems = source.GetBeverageLineItems().Count();
        }
    }
}
