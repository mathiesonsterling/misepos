using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Database.AzureDefinitions.ValueItems
{
    [ComplexType]
    public class EventID : Core.ValueItems.EventID, IDbValueItem<Core.ValueItems.EventID>
    {
        public EventID() { }

        public EventID(Core.ValueItems.EventID source)
        {
            AppInstanceCode = source.AppInstanceCode;
            OrderingID = source.OrderingID;
        }

        public Core.ValueItems.EventID ToValueItem()
        {
            return this;
        }
    }
}
