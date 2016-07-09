namespace Mise.Core.Client.Entities.Restaurant
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
            MiseApplicationName = app.Name;
        }

        public Restaurant Restaurant { get; set; }
	    public string RestaurantId { get; set; }

        public MiseApplication MiseApplication { get; set; }

        public string MiseApplicationName { get; set; }
    }
}
