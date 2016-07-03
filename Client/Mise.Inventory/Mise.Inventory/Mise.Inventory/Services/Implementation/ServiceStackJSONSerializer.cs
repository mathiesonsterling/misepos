using System;
using System.Threading.Tasks;

//using ServiceStack.Text;
using Mise.Core.Services.UtilityServices;
namespace Mise.Inventory.Services.Implementation
{
	/*
	/// <summary>
	/// Quick class to let us use the servicestack serializer (advertised as 3X quicker than JSON.NET on mono)
	/// </summary>
	public class ServiceStackJSONSerializer : IJSONSerializer
	{
		#region IJSONSerializer implementation

		public T Deserialize<T> (string json) where T : class
		{
			return JsonSerializer.DeserializeFromString<T> (json);
		}

		public object Deserialize (string json, Type type)
		{
			return JsonSerializer.DeserializeFromString (json, type);
		}

		public string Serialize<T> (T obj)
		{
			return JsonSerializer.SerializeToString (obj);
		}

		public object Deserialize (System.IO.Stream stream, Type type)
		{
			return JsonSerializer.DeserializeFromStream (type, stream);
		}

		public Task<T> DeserializeAsync<T> (string json) where T : class
		{
			return Task.Run(() => Deserialize<T>(json));
		}

		#endregion
	}*/
}

