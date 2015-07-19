using System;
using System.Threading.Tasks;
using Mise.Core.Client.Services;
using Mise.Core.Services.UtilityServices;
using System.Collections.Generic;


namespace Mise.Inventory
{
	/// <summary>
	/// Currently set just to memory, till I can figure out why this crap keeps crashing
	/// </summary>
	public class CrossSettingsClientKeyValueStorage : IClientKeyValueStorage
	{
		readonly IJSONSerializer _serial;
		readonly Dictionary<string, string> _dic;
		public CrossSettingsClientKeyValueStorage(IJSONSerializer serial){
			_serial = serial;
			_dic = new Dictionary<string, string> ();
			
		}
		#region IClientKeyValueStorage implementation

		public void SetValue<T> (string key, T value) where T:class
		{
			_dic [key] = _serial.Serialize (value);
		}

		public T GetValue<T> (string key) where T:class
		{
			if(_dic.ContainsKey (key) == false){
				return default(T);
			}

			var json = _dic [key];
			return _serial.Deserialize<T> (json);
		}


		public void DeleteValue (string key)
		{
			_dic.Remove (key);
		}
		#endregion
	}
}

