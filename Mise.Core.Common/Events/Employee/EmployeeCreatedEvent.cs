using Mise.Core.Entities;
using Mise.Core.ValueItems;

namespace Mise.Core.Common.Events.Employee
{
    public class EmployeeCreatedEvent : BaseEmployeeEvent
    {
        public override MiseEventTypes EventType
        {
            get { return MiseEventTypes.EmployeeCreatedEvent; }
        }

        public override bool IsAggregateRootCreation
        {
            get { return true; }
        }

        public override bool IsEntityCreation
        {
            get { return true; }
        }

        public EmailAddress Email{get;set;}
        public PersonName Name { get; set; }

		public Password Password{get;set;}

        /// <summary>
        /// The type of app that's creating this employee
        /// </summary>
        public MiseAppTypes AppType { get; set; }

		/// <summary>
		/// If set, the value of the token from the external login we're using
		/// </summary>
		/// <value>The O auth token.</value>
		public OAuthToken OAuthToken{get;set;}
    }
}
