using Mise.Core.ValueItems;

namespace Mise.Database.StorableEntities.ValueItems
{
    public class BeaconGraphNode
    {
        public BeaconGraphNode() { }

        public BeaconGraphNode(Beacon source)
        {
            UUID = source.UUID;
            Major = source.Major;
            Minor = source.Minor;
            if (source.LocationPlaced != null)
            {
                LongitudePlacedAt = source.LocationPlaced.Longitude;
                LatitudePlacedAt = source.LocationPlaced.Latitude;
            }
        }

        public Beacon Rehydrate()
        {
            var b = new Beacon
            {
                UUID = UUID,
                Major = Major,
                Minor = Minor,
            };

            if (LongitudePlacedAt.HasValue && LatitudePlacedAt.HasValue)
            {
                b.LocationPlaced = new Location
                {
                    Latitude = LatitudePlacedAt.Value,
                    Longitude = LongitudePlacedAt.Value

                };
            }

            return b;
        }

        public string Minor{ get; set; }

        public string Major { get; set; }

        public string UUID { get; set; }

        public double? LongitudePlacedAt { get; set; }
        public double? LatitudePlacedAt { get; set; }
    }
}
