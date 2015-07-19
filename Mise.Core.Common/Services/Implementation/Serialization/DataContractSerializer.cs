using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Entities.MenuItems;
using Mise.Core.Entities.Menu;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Common.Services.Implementation.Serialization
{
    public class DataContractSerializer : IJSONSerializer
    {
        private List<Type> _knownTypes;

        public DataContractSerializer()
        {
            _knownTypes = new List<Type>
            {
                typeof(MenuItem),
                typeof(MenuItemCategory)

            };
        }
		public T Deserialize<T>(string json) where T:class
        {
            var ser = new DataContractJsonSerializer(typeof(T), _knownTypes);
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return (T) ser.ReadObject(stream);
            }
        }

        public object Deserialize(string json, Type type)
        {
            var ser = new DataContractJsonSerializer(type, _knownTypes);
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return ser.ReadObject(stream);
            }
        }

        public object Deserialize(Stream stream, Type type)
        {
            var ser = new DataContractJsonSerializer(type, _knownTypes);
            return ser.ReadObject(stream);
        }

        public Task<T> DeserializeAsync<T>(string json) where T:class
        {
            throw new NotImplementedException();
        }

        public Task<object> DeserializeAsync(string json, Type type)
        {
            throw new NotImplementedException();
        }

        public Task<string> SerializeAsync<T>(T obj)
        {
            throw new NotImplementedException();
        }

        public Type GetConcreteTypeForInterface(Type interfaceType)
        {
            throw new NotImplementedException();
        }

        public string Serialize<T>(T obj)
        {
            var ser = new DataContractJsonSerializer(typeof (T), _knownTypes);
            using (var stream = new MemoryStream())
            {
                ser.WriteObject(stream, obj);

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                } 
            }
        }


    }
}
