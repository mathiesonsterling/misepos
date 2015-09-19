using System;

using Mise.Core.ValueItems;
using Mise.Core.Services.UtilityServices;
using Mindscape.Raygun4Net;

namespace Mise.Inventory.iOS.Services
{
	public class RaygunErrorTracking : IErrorTrackingService
	{
		private RaygunClient _raygunClient;
		public RaygunErrorTracking ()
		{
			_raygunClient = new RaygunClient ("2ZV9A+X5sb5dNz4klhTD8A==");
		}

		#region IErrorTrackingService implementation

		public void ReportException (Exception e, LogLevel level)
		{
			_raygunClient.SendInBackground (e);
		}

		public void Identify (Guid? userID, EmailAddress email, PersonName name, string deviceID, bool isAnonymous)
		{
			var emailString = email != null ? email.Value : string.Empty;
			_raygunClient.UserInfo = new Mindscape.Raygun4Net.Messages.RaygunIdentifierMessage (emailString) {
				FullName = name != null ? name.ToSingleString () : string.Empty,
				FirstName = name != null ? name.FirstName : string.Empty,
				Identifier = userID.HasValue ? userID.Value .ToString (): string.Empty,
				IsAnonymous = isAnonymous,
				UUID = deviceID
			};
		}

		public void Identify (Mise.Core.Entities.People.IEmployee employee, string deviceID)
		{
			if(employee != null){
				Identify (employee.Id, employee.PrimaryEmail, employee.Name, deviceID, false);
			} else {
				Identify (null, null, null, deviceID, true);
			}
		}

		#endregion
	}
}

