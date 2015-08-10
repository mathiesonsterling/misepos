using System;
using System.Threading.Tasks;

using Mise.Core.Client.Services;
using Mise.Core.Services.UtilityServices;

using Xamarin.Forms;
namespace Mise.Inventory
{
	public class XamarinFormsSimpleKeyValueStorage : IClientKeyValueStorage
	{
		readonly IJSONSerializer _serializer;
		public XamarinFormsSimpleKeyValueStorage (IJSONSerializer serializer)
		{
			_serializer = serializer;
		}

		#region IClientKeyValueStorage implementation

		public async Task SetValue<T> (string key, T value) where T : class
		{
			var json = _serializer.Serialize (value);
			Application.Current.Properties [key] = json;

			await Application.Current.SavePropertiesAsync();
		}

		public T GetValue<T> (string key) where T : class
		{
			if (Application.Current.Properties.ContainsKey (key) == false) {
				return default(T);
			}

			var json = Application.Current.Properties [key].ToString();
			return _serializer.Deserialize<T> (json);
		}

		public async Task DeleteValue (string key)
		{
			if (Application.Current.Properties.ContainsKey (key)) {
				Application.Current.Properties.Remove (key);
				await Application.Current.SavePropertiesAsync ();
			}
		}

		public async Task SetID(string key, Guid id){
			Application.Current.Properties [key] = id.ToString ();
			await Application.Current.SavePropertiesAsync ();
		}

		public Guid? GetID(string key){
			if(Application.Current.Properties.ContainsKey (key) == false){
				return null;
			}

			return Guid.Parse (Application.Current.Properties [key].ToString ());
		}
		#endregion
	}
}

