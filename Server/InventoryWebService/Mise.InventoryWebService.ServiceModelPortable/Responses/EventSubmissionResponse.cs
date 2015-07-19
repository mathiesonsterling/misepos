namespace Mise.InventoryWebService.ServiceModelPortable.Responses
{
    /// <summary>
    /// Class for when we submit events to any place
    /// </summary>
    public class EventSubmissionResponse
    {
        public int NumEventsProcessed { get; set; }

        public bool Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}
