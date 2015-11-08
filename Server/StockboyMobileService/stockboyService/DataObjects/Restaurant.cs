﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using Mise.Core.Entities;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Inventory;
using Mise.Core.Entities.Payments;
using Mise.Core.Entities.Restaurant;
using Mise.Core.Entities.Restaurant.Events;
using Mise.Core.ValueItems;

namespace stockboyService.DataObjects
{
    public class Restaurant : EntityData
    {
        public DateTimeOffset CreatedDate { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public Guid MiseId { get; set; }

        public EventID Revision { get; set; }

        public Guid RestaurantID { get; set ;}

        public Guid? AccountID { get; set; }
        public RestaurantName Name { get; set; }
        public StreetAddress StreetAddress { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        public int NumberOfActiveCashRegisters { get; set; }
        public int NumberOfActiveCreditRegisters { get; set; }
        public int NumberOfActiveOrderTerminals { get; set; }
        public string FriendlyID { get; set; }

        public Uri RestaurantServerLocation { get; set; }
        public bool IsPlaceholder { get; set; }
        public Guid? CurrentInventoryID { get; set; }
        public Guid? LastMeasuredInventoryID { get; set; }
    }
}
