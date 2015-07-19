using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Core.Common.Services.Implementation.Serialization;
using Mise.Core.Common.UnitTests.Entities.Serialization;
using Mise.Core.Services.UtilityServices;

namespace Mise.Core.Common.UnitTests.Tools
{
    public static class SerializerFactory
    {
        public static IJSONSerializer GetJSONSerializer(SerializationType type)
        {
            if (type == SerializationType.JSONDOTNET)
            {
                return new JsonNetSerializer();
            }
            
            throw new ArgumentException("Don't have a serializer for " + type);
        }
    }
}
