using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Entities;

namespace Mise.Core.Client.ValueItems
{
    public class EventIDDb : Core.ValueItems.EventID, IDbValueItem<Core.ValueItems.EventID>
    {
        public EventIDDb() { }

        public EventIDDb(Core.ValueItems.EventID source)
        {
            if (source != null)
            {
                AppInstanceCode = source.AppInstanceCode;
                OrderingID = source.OrderingID;
            }
        }

        public Core.ValueItems.EventID ToValueItem()
        {
            return this;
        }
    }
}
