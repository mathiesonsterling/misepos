using System;
using System.Net;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using Newtonsoft.Json;

namespace Mise.Core.Server.Windows.Services.Implementation
{
    public class GoogleMapsGeoCodingService : IGeoCodingService
    {

        public async Task<Location> GetLocationForAddress(StreetAddress address)
        {
            const string GOOGLEMAPSURL = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true&address=";

            var addressString = address.ToSingleString();
            var uri = new Uri(GOOGLEMAPSURL + addressString);
            using (var webClient = new WebClient())
            {
                var rawResponse = await webClient.DownloadStringTaskAsync(uri);
                dynamic parsedJson = JsonConvert.DeserializeObject(rawResponse);

                var results = parsedJson.results;
                if (results == null)
                {
                    return null;
                }
                var firstRes = results[0];
                if (firstRes == null || firstRes.geometry == null || firstRes.geometry.location == null)
                {
                    return null;
                }
                var location = new Location
                {
                    Latitude = firstRes.geometry.location.lat,
                    Longitude = firstRes.geometry.location.lng
                };
                return location;
            }
        }
    }
}
