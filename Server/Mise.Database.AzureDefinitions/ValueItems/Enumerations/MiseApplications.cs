using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server;
using Mise.Core.Entities;

namespace Mise.Database.AzureDefinitions.ValueItems.Enumerations
{
    public class MiseApplication : EntityData
    {
        public MiseApplication() { }

        public MiseApplication(MiseAppTypes type)
        {
            AppTypeValue = (int) type;
            Name = type.ToString();
        }

        public MiseAppTypes ToEnum()
        {
            return (MiseAppTypes) AppTypeValue;
        }

        public int AppTypeValue { get; set; }
        public string Name { get; set; }
    }
}
