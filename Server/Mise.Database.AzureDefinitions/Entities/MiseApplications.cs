using System;
using Microsoft.Azure.Mobile.Server;
using Mise.Core.Entities;

namespace Mise.Database.AzureDefinitions.Entities
{
    public class MiseApplication : EntityData
    {
        public MiseApplication() { }

        public MiseApplication(MiseAppTypes type)
        {
            AppTypeValue = (int) type;
            Name = type.ToString();
            Id = AppTypeValue.ToString();
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public MiseAppTypes ToEnum()
        {
            return (MiseAppTypes) AppTypeValue;
        }

        public int AppTypeValue { get; set; }
        public string Name { get; set; }
    }
}
