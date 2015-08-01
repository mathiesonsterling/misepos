using System;
using Mise.Core.Entities;
using Mise.Core.Common.Entities.DTOs;
using Mise.Core.Common.Entities;


namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
	/// <summary>
	/// Quick non-typed class for storage of any entity
	/// </summary>
	public class AzureEntityStorage
	{
		public AzureEntityStorage(){}

		public AzureEntityStorage(RestaurantEntityDataTransportObject dto, BuildLevel level){
			BuildLevel = level.ToString ();
			MiseEntityType = dto.SourceType.ToString ();
			EntityID = dto.ID;
			RestaurantID = dto.RestaurantID;
			JSON = dto.JSON;
			LastUpdatedDate = dto.LastUpdatedDate;
		}

		public RestaurantEntityDataTransportObject ToRestaurantDTO(){
			return new RestaurantEntityDataTransportObject {
				SourceType = Type.GetType (MiseEntityType),
				ID = EntityID,
				RestaurantID = RestaurantID,
				JSON = JSON,
				LastUpdatedDate = LastUpdatedDate
			};
		}

		public string BuildLevel{ get; set;}
		public string MiseEntityType{get;set;}
		public Guid EntityID{ get; set;}
		public Guid? RestaurantID{ get; set;}
		public string JSON{get;set;}
		public DateTimeOffset LastUpdatedDate{get;set;}
	}
}

