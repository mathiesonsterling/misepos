using System;
using Mise.Core.Common.Events.DTOs;

namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	public class AzureEventStorage
	{
		public AzureEventStorage(){}

		public AzureEventStorage(EventDataTransportObject dto, BuildLevel level){
			BuildLevel = level.ToString ();
			MiseEventType = dto.EventType.ToString ();
			EventID = dto.ID;
			id = dto.ID.ToString();
			EventDate = dto.CreatedDate;
			JSON = dto.JSON;
		}

		public string id{get;set;}
		public string BuildLevel{get;set;}
		public string MiseEventType{get;set;}
		public Guid EventID{get;set;}
		public DateTimeOffset EventDate{get;set;}
		public string JSON{get;set;}
	}
}

