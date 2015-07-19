using System;

namespace Mise.Core.ValueItems.Inventory
{
	/// <summary>
	/// Represents an amount of liquid.  Used for containers, measuring how much of a liquid is in inventory or a recipe, etc
	/// </summary>
	public class LiquidAmount : IEquatable<LiquidAmount>
	{
	    public static LiquidAmount Empty
	    {
	        get { return new LiquidAmount {Milliliters = 0, SpecificGravity = null}; }
	    }

	    /// <summary>
        /// Only this is actually stored, but we should use the methods.  Interface it.
        /// </summary>
        public decimal Milliliters { get; set; }

        /// <summary>
        /// The density of the liquid - used to allow us to measure it by weight
        /// </summary>
        public decimal? SpecificGravity { get; set; }

        /// <summary>
        /// Multiply this amount by an argument
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
	    public LiquidAmount Multiply(decimal arg)
        {
            return new LiquidAmount
            {
                Milliliters = Milliliters*arg
            };
        }
        /// <summary>
        /// Lets us know if this is a zero amount of liquid
        /// </summary>
	    public bool IsEmpty
	    {
	        get { return Milliliters > 0; }
	    }

	    /// <summary>
	    /// Amout represented in ML
	    /// </summary>
	    /// <value>The milliliters.</value>
	    public decimal GetInMilliliters()
	    {
			return Milliliters;
	    }

	    /// <summary>
	    /// Amount in liquid OZ
	    /// </summary>
	    /// <value>The ounces.</value>
	    public decimal GetInLiquidOunces()
	    {
			return Milliliters / 29.5735M;
	    }

	    public static LiquidAmount FromLiquidOunces(decimal ounces)
	    {
	        var inML = ounces*29.573M;
	        return new LiquidAmount {Milliliters = inML};
	    }

	    /// <summary>
	    /// Gets how much this liquid is by weight
	    /// </summary>
	    /// <value>The weight</value>
	    public Weight GetWeight()
	    {
	        if (SpecificGravity.HasValue == false)
	        {
	            throw new InvalidOperationException("Specific gravity must be set to get the weight of a LiquidAmount!");
	        }
	        var grams = Milliliters*SpecificGravity.Value;
	        return new Weight {Grams = grams};
	    }

	    public bool Equals(LiquidAmount other)
	    {
	        if (ReferenceEquals(null, other)) return false;
	        if (ReferenceEquals(this, other)) return true;
	        return Milliliters == other.Milliliters && SpecificGravity == other.SpecificGravity;
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((LiquidAmount) obj);
	    }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Milliliters.GetHashCode() * 397) ^ SpecificGravity.GetHashCode();
            }
        }
	}
}

