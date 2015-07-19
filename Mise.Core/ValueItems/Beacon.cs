using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    /// <summary>
    /// Represents iBeacons (or android equiv) broadcasting their location
    /// </summary>
    public class Beacon : IEquatable<Beacon>
    {
        public string UUID { get; set; }
        public string Major { get; set; }
        public string Minor { get; set; }

        /// <summary>
        /// If known, where this beacon is physically located.  We will have entered this, the beacon doesn't know!
        /// </summary>
        public Location LocationPlaced { get; set; }

        public bool Equals(Beacon other)
        {
            return UUID.Equals(other.UUID)
                   && Major.Equals(other.Major)
                   && Minor.Equals(other.Minor);
        }
    }
}
