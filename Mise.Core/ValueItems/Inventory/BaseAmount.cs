using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.ValueItems.Inventory
{
    /// <summary>
    /// Represents the quantity of some amount
    /// </summary>
    public abstract class BaseAmount
    {
        //The number of how much we're talking about
        public decimal Value { get; protected set; }

        /// <summary>
        /// What unit value is in
        /// </summary>
        public AmountUnits StoredUnit { get; }

        /// <summary>
        /// The type of thing we are
        /// </summary>
        public AmountTypes Type { get; }

        /// <summary>
        /// How to make a new unit.  Usually just base class constructor
        /// </summary>
        protected abstract Func<decimal, BaseAmount> MakeNew { get; }

        /// <summary>
        /// Multiply this amount by an argument
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public BaseAmount Multiply(decimal arg)
        {
            return MakeNew(Value = Value*arg);
        }

        public decimal Divide(BaseAmount other)
        {
            return Value / other.Value;
        }

        public BaseAmount Add(BaseAmount other)
        {
            CheckMatchedUnit(other);
            return MakeNew(Value = Value + other.Value);
        }

        public bool GreaterThan(BaseAmount other)
        {
            CheckMatchedUnit(other);
            return Value > other.Value;
        }

        public BaseAmount Subtract(BaseAmount other)
        {
            CheckMatchedUnit(other);
            return MakeNew(Value = Value - other.Value);
        }

        private void CheckMatchedUnit(BaseAmount other)
        {
            if (Type != other.Type)
            {
                throw new ArgumentException($"Types {Type} and {other.Type} do not match!");
            }

            if (StoredUnit != other.StoredUnit)
            {
                throw new ArgumentException($"Units {StoredUnit} and {other.StoredUnit} do not match!");
            }
        }

        /// <summary>
        /// Lets us know if this is a zero amount of liquid
        /// </summary>
	    public bool IsEmpty => Value <= 0;

    }
}
