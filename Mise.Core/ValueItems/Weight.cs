using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems
{
    /// <summary>
    /// Class that represents a measure of weight
    /// </summary>
    public class Weight : IEquatable<Weight>
    {

        public decimal Grams { get; set; }
        public bool Equals(Weight other)
        {
            return Grams.Equals(other.Grams);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Weight;
            if (other == null)
            {
                return false;
            }
            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Grams.GetHashCode();
        }
    }
}
