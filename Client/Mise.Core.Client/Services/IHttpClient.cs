using System;

using System.Threading.Tasks;
namespace Mise.Core.Client
{
	/// <summary>
	/// Basis interface for anything talking over the wire
	/// Abstracted for platform specific ones to be used
	/// </summary>
	public interface IHttpClient
	{
		Task<string> GetAsync (Uri address);
		Task<bool> PutAsync(Uri address, string data);
		Task<bool> PostAsync(Uri address, string data);
	}
}

