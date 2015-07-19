using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Core.Services.UtilityServices
{
    public interface IJSONSerializer
    {
		T Deserialize<T>(string json) where T:class;

        object Deserialize(string json, Type type);

        string Serialize<T>(T obj);

        object Deserialize(Stream stream, Type type);
        Task<T> DeserializeAsync<T>(string json) where T : class;
    }
}
