using System;

namespace Mise.Core.ValueItems
{
	public class Money : IEquatable<Money>, ITextSearchable
	{
		public decimal Dollars{ get; set;}
		public Money (decimal amountInDollars)
		{
			Dollars = Math.Round (amountInDollars, 2);
		}

		public Money(){

		}

		/// <summary>
		/// Add two amounts of money
		/// </summary>
		/// <param name="other">Other.</param>
		public Money Add(Money other){
			return new Money (Dollars + other.Dollars);
		}

		public Money Subtract(Money other){
			return new Money (Dollars - other.Dollars);
		}

		/// <summary>
		/// Multiple this amount by another
		/// </summary>
		/// <param name="ratio">Ratio.</param>
		public Money Multiply(decimal ratio){
			return new Money (Dollars * ratio);
		}

		public bool GreaterThanOrEqualTo(Money other){
			return Dollars >= other.Dollars;
		}

		public bool GreaterThan(Money other){
			return Dollars > other.Dollars;
		}

		public bool HasValue{ get { return Dollars > 0.0M;} }

		public string ToFormattedString(){
			return Dollars.ToString ("C");
		}

		public override bool Equals (object obj)
		{
			var mon = obj as Money;
			if (mon == null) {
				return false;
			}

			return Dollars == mon.Dollars;
		}

		public bool Equals (Money other)
		{
			return Dollars == other.Dollars;
		}


		public override int GetHashCode ()
		{
			return Dollars.GetHashCode ();
		}

	    public bool ContainsSearchString(string searchString)
	    {
	        return Dollars.ToString().Contains(searchString);
	    }

	    public static Money None{
			get{
				return new Money (0.0M);
			}
		}
	}
}

