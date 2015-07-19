using System;
using System.IO;
using System.Threading.Tasks;
using Mise.Core.Services.UtilityServices;
using Mise.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Mise.Core.Entities.Base;
using Mise.Core.Entities.Menu;
using System.Runtime.Serialization;



namespace Mise.Core.Common.Services.Implementation.Serialization
{
	public class JsonNetSerializer : IJSONSerializer
    {
		public T Deserialize<T> (string json) where T:class
		{
			return Deserialize (json, typeof(T)) as T;
		}

	    public Task<T> DeserializeAsync<T>(string json) where T : class
	    {
	        return Task.Run(() => Deserialize<T>(json));
	    }

        public object Deserialize(string json, Type type)
        {

           return JsonConvert.DeserializeObject(json, type);
            
        }


        public object Deserialize(Stream stream, Type type)
        {
            var sr = new StreamReader(stream);
            var jreader = new JsonTextReader(sr);

            var ser = new JsonSerializer();
            ser.Converters.Add(new IsoDateTimeConverter());

            var val = ser.Deserialize(jreader, type);
            return val;
        }


        public string Serialize<T>(T obj)
        {
        	return JsonConvert.SerializeObject(obj);
        }

		public string Serialize<T>(T obj, ILogger logger){
			try{
				return JsonConvert.SerializeObject(obj);
			} catch(Exception e){
				logger.HandleException (e);
				throw;
			}
		}
    }
}

