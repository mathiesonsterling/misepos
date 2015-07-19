using Mise.Core.Entities;

namespace Mise.Core.Common.Events.Checks
{
    public class CheckCreatedEvent : BaseCheckEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.CheckCreated;}
        }
    }
}

