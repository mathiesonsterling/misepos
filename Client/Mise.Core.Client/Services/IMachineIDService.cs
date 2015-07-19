using System;

namespace Mise.Core.Client.Services
{
	/// <summary>
	/// Gets a string that uniquely identifies a device
	/// </summary>
	public interface IMachineIDService
	{
		string GetID();
	}
}

