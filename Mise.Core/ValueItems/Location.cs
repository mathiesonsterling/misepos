using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    /// <summary>
    /// A physical location of something
    /// </summary>
    public class Location : IEquatable<Location>
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public bool Equals(Location other)
        {
            return Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
        }
    }
}
