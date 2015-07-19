using System;

/// <summary>
/// Represents where a order should send to notify.  Could be a printer, a terminal, many things
/// </summary>
namespace Mise.Core.ValueItems
{
	public interface IOrderDestination
	{
		string ID{ get;}
		string Name{ get; }
	}
}

