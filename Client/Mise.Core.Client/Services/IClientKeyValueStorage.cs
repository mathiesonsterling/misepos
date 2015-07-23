using System;
using System.Threading.Tasks;
namespace Mise.Core.Client.Services
{
	/// <summary>
	/// Basic persistant storage for string keys and values
	/// </summary>
	public interface IClientKeyValueStorage
	{
		Task SetValue<T>(string key, T value) where T:class;
		T GetValue<T>(string key) where T:class;
		Task DeleteValue(string key);
	}
}

