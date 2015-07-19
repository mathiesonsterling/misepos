using System;

namespace Mise.Core.ValueItems
{
    /// <summary>
    /// Class to represent how far apart two things are
    /// </summary>
    public class Distance : IEquatable<Distance>, IComparable<Distance>
    {
        #region Constructors

        public Distance()
        {
			Kilometers = 0;
        }

        /// <summary>
        /// Construct this distance, based upon two points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        public Distance(Location point1, Location point2)
        {
			Kilometers = DistanceBetweenPlaces (point1.Longitude, point1.Latitude, point2.Longitude, point2.Latitude);
        }

        #region Distance calculation
        const double RADIUS = 6378.16;
        /// <summary>
        /// Calculate the distance between two places, return the value in KM
        /// </summary>
        /// <param name="lon1"></param>
        /// <param name="lat1"></param>
        /// <param name="lon2"></param>
        /// <param name="lat2"></param>
        /// <returns></returns>
        public static double DistanceBetweenPlaces(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            var dlon = Radians(lon2 - lon1);
            var dlat = Radians(lat2 - lat1);

            var a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(Radians(lat1)) * Math.Cos(Radians(lat2)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            var angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }

        /// <summary>
        /// Convert degrees to Radians
        /// </summary>
        /// <param name="x">Degrees</param>
        /// <returns>The equivalent in radians</returns>
        public static double Radians(double x)
        {
            return x * Math.PI / 180;
        }
        #endregion
        #endregion

        public double Kilometers { get; set; }

        public double InMiles()
        {
            return Kilometers / 1.60934;
        }

        public bool Equals(Distance other)
        {
			if (other != null) {
				return Kilometers.Equals (other.Kilometers);
			}
			return false;
        }

        public bool GreaterThan(Distance other)
        {
			if (other != null) {
				return Kilometers > other.Kilometers;
			}
			return true;
        }
        public int CompareTo(Distance other)
        {
            return Kilometers.CompareTo(other.Kilometers);
        }
    }
}
