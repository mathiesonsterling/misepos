﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using Mise.Core.Common.Entities.DTOs;
using Newtonsoft.Json;

namespace stockboymobileserviceService.DataObjects
{
    /// <summary>
    /// Weak storage on entities - holds their type and JSON, not much else
    /// </summary>
    public class AzureEntityStorage : EntityData
    {
        public string BuildLevel { get; set; }

        public string MiseEntityType { get; set; }

        public Guid EntityID { get; set; }

        public Guid? RestaurantID { get; set; }

        public string EntityJSON { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

    }
}
