using System;
using Mise.Core.Common.Entities.DTOs;
namespace Mise.Inventory.Services.Implementation.WebServiceClients.Azure
{
    public class AzureEntityWithLocationStorage : AzureEntityStorage
    {
        public AzureEntityWithLocationStorage(){
        }

        public AzureEntityWithLocationStorage(RestaurantEntityDataTransportObject dto, double longitude, double latitude) :
        base(dto){
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

    }
}

