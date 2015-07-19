using System;

namespace Mise.Core.ValueItems
{
	public class OrderDestination : IEquatable<OrderDestination>
	{
		public string Name {
			get;
			set;
		}

	    public bool Equals(OrderDestination other)
	    {
	        return Name.Equals(other.Name);
	    }
	}
}

