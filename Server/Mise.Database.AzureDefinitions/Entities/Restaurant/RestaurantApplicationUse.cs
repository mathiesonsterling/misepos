using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Server;

namespace Mise.Database.AzureDefinitions.Entities.Restaurant
{
    /// <summary>
    /// Represents that a restaurant can use an application
    /// </summary>
    public class RestaurantApplicationUse : EntityData
    {
        public RestaurantApplicationUse() { }

        public RestaurantApplicationUse(Restaurant rest, MiseApplication app)
        {
            Id = rest.EntityId + ":" + app.AppTypeValue;
        }

        public Restaurant Restaurant { get; set; }
	    [ForeignKey("Restaurant")]
	    public string RestaurantId { get; set; }

        public MiseApplication MiseApplication { get; set; }
    }
}
