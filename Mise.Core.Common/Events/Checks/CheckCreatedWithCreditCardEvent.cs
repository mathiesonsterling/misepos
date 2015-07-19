using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Checks
{
    public class CheckCreatedWithCreditCardEvent : CheckCreatedEvent
    {
        public CreditCard CreditCard { get; set; }

        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.CheckCreatedWithCreditCard; }
        }
    }
}
