using Newtonsoft.Json;
using SharedModels.EventStoreCQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventStoreCQRS
{
    public class EventDeserializer
    {
        private readonly JsonSerializerSettings settings;

        public EventDeserializer()
        {
            settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public IEvent Deserialize(byte[] utf8Bytes)
        {
            string jsonData = Encoding.UTF8.GetString(utf8Bytes);
            return JsonConvert.DeserializeObject<IEvent>(jsonData, settings);
        }
    }
}
