﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.Inventory;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.People;

namespace MiseReporting.Models
{
    public class InventoryViewModel
    {
        public DateTime? DateCompleted { get; set; }

        public DateTime DateCreated { get; set; }

        public string DoneByEmployee { get; set; }
        public Guid Id { get; set; }

        public bool HasLineItems { get; set; }

        public InventoryViewModel() { }

        public InventoryViewModel(IInventory source, IEmployee emp)
        {
            DateCompleted = source.DateCompleted?.LocalDateTime;

            if (emp != null)
            {
                DoneByEmployee = emp.DisplayName;
            }

            DateCreated = source.CreatedDate.ToLocalTime().LocalDateTime;
            Id = source.ID;
            HasLineItems = source.GetBeverageLineItems().Any();
        }
    }
}