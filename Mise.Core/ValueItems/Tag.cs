using System;

namespace Mise.Core.ValueItems
{
	/// <summary>
	/// Basic tag
	/// </summary>
	public class Tag : IEquatable<Tag>
	{
		public string Name{get;set;}

		#region IEquatable implementation

		public bool Equals (Tag other)
		{
			return other != null && Name.Equals (other.Name);
		}

		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			var other = obj as Tag;
			return other != null && other.Name.Equals (Name);
		}
		#endregion
	}
}

