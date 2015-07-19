using System;
using System.Collections.Generic;
using Mise.Core.Common.Events.DTOs;
using ServiceStack;

namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    /// <summary>
    /// Used to submit any events we have
    /// </summary>
    [Route("/events")]
    public class EventSubmission : IReturn<EventSubmissionResponse>
    {
        /// <summary>
        /// Restaurant these events belong to
        /// </summary>
        public Guid RestaurantID { get; set; }

        /// <summary>
        /// The wrapped up events we want to give to the server
        /// </summary>
        public IEnumerable<EventDataTransportObject> Events { get; set; }

        /// <summary>
        /// The device submitting the events
        /// </summary>
        public string DeviceID { get; set; }
    }
}