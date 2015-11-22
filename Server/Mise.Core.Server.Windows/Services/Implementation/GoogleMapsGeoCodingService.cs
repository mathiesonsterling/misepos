using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.ValueItems;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace Mise.Core.Server.Windows.Services.Implementation
{
    public class GoogleMapsGeoCodingService : IGeoCodingService
    {

        public Task<Location> GetLocationForAddress(StreetAddress address)
        {
            const string GOOGLEMAPSURL = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true&address=";

            var addressString = address.ToSingleString();
            var uri = new Uri(GOOGLEMAPSURL + addressString);
            using (var webClient = new WebClient())
            {
                var rawResponse = webClient.DownloadString(uri);
                dynamic parsedJson = JsonConvert.DeserializeObject(rawResponse);

                var results = parsedJson.results;
                if (results != null)
                {
                    var firstRes = results[0];
                    if (firstRes != null && firstRes.geometry != null && firstRes.geometry.location != null)
                    {
                        var location = new Location
                        {
                            Latitude = firstRes.geometry.location.lat,
                            Longitude = firstRes.geometry.location.lng
                        };
                        return Task.FromResult(location);
                    }
                }
            }
            Location nullLoc = null;
            return Task.FromResult(nullLoc);
        }
    }
}
