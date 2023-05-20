using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharedModels.EventStoreCQRS;

namespace Common.EventStoreCQRS
{
    public class EventSerializer
    {
        private readonly JsonSerializerSettings settings;

        public EventSerializer()
        {
            settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public byte[] Serialize<TEvent>(TEvent eventInstance) where TEvent : IEvent
        {
            string jsonData = JsonConvert.SerializeObject(eventInstance, settings);
            return Encoding.UTF8.GetBytes(jsonData);
        }
    }
}
