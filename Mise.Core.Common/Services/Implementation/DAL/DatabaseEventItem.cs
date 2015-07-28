using System;
using Mise.Core.Common.Events.DTOs;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Services.Implementation.DAL
{
    /// <summary>
    /// Stores our events 
    /// </summary>
	public class DatabaseEventItem : EventDataTransportObject
	{
        public DatabaseEventItem()
        {
            TimesAttemptedToSend = 0;
        }

        public DatabaseEventItem(EventDataTransportObject source, bool sent) : base(source)
        {
            HasBeenSent = sent;
            TimesAttemptedToSend = 0;

        }

        public bool HasBeenSent { get; set; }
        public int TimesAttemptedToSend { get; set; }
	}
}

