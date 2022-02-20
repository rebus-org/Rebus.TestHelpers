using Newtonsoft.Json;
using Rebus.Sagas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rebus.TestHelpers.Internals
{
    internal class NewtonSoftSagaSerializer : ISagaSerializer
    {

        readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
        };
        public ISagaData DeserializeFromString(Type type, string str)
        {
            return (ISagaData)JsonConvert.DeserializeObject(str, type, _serializerSettings);
        }

        public string SerializeToString(ISagaData obj)
        {
            return JsonConvert.SerializeObject(obj, _serializerSettings);
        }
    }
}
