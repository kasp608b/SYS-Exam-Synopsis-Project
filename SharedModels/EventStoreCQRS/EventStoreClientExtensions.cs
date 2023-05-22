using Common.EventStoreCQRS;
using EventStore.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedModels.EventStoreCQRS
{
    public static class EventStoreClientExtensions
    {
        public static async Task<TAggregate?> Find<TAggregate, TId>(this EventStoreClient eventStore, Guid id, EventDeserializer eventDeserializer, CancellationToken cancellationToken)
    where TAggregate : Aggregate<TId>, new()
        {
          
            var readResult = eventStore.ReadStreamAsync(
                Direction.Forwards,
                $"{typeof(TAggregate).Name}-{id}",
                StreamPosition.Start,
                StreamState.StreamExists,
                cancellationToken: cancellationToken
            );

            if (await readResult.ReadState == ReadState.StreamNotFound)
            {
                return null;
            }

            var aggregate = new TAggregate();

            await foreach (var @event in readResult)
            {
                try
                {
                    if (@event.Event.EventType.StartsWith("$") || @event.Event.EventType.StartsWith("_"))
                    {
                        continue; // skip system events
                    }

                    var eventData = eventDeserializer.Deserialize(@event.Event.Data.ToArray());

                    aggregate.When(eventData!);
                }
                catch (JsonSerializationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }

            return aggregate;
        }

        public static async Task Append<TEvent>(this EventStoreClient eventStore, TEvent @event, string aggregateType, EventSerializer eventSerializer ,CancellationToken cancellationToken) where TEvent : IEvent
        {
            var eventData = new EventData(
                Uuid.NewUuid(),
                @event.GetType().Name,
                eventSerializer.Serialize(@event)
            );

            await eventStore.AppendToStreamAsync(
                $"{aggregateType}-{@event.Id}",
                StreamState.Any,
                new[] { eventData },
                cancellationToken: cancellationToken
            );
        }

        /*
        private static TEvent Deserialize<TEvent>(ResolvedEvent @event) where TEvent : IEvent
        {
            
            var type = Type.GetType(@event.Event.EventType);

            if (type == null)
            {
                throw new InvalidOperationException($"Cannot find type '{@event.Event.EventType}'");
            }
            

            var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());

            var deserializedEvent = JsonSerializer.Deserialize<TEvent>(json);

            if (deserializedEvent == null)
            {
                throw new InvalidOperationException($"Could not deserialize '{@event.Event.EventType}'");
            }

            return deserializedEvent;
        }

        private static object Deserialize(ResolvedEvent @event)
        {
            
            var type = Type.GetType(@event.Event.EventType);

            if (type == null)
            {
                throw new InvalidOperationException($"Cannot find type '{@event.Event.EventType}'");
            }

            var json = Encoding.UTF8.GetString(@event.Event.Data.ToArray());

            var deserializedEvent = JsonSerializer.Deserialize(json, type);

            if (deserializedEvent == null)
            {
                throw new InvalidOperationException($"Could not deserialize '{@event.Event.EventType}'");
            }

            return deserializedEvent;
        }
        */
    }
}
