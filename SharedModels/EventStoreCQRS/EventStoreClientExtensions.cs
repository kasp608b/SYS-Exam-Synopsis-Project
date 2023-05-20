﻿using EventStore.Client;
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
        public static async Task<TAggregate?> Find<TAggregate, TId>(this EventStoreClient eventStore, Guid id, CancellationToken cancellationToken)
    where TAggregate : Aggregate<TId>, new()
        {
            var readResult = eventStore.ReadStreamAsync(
                Direction.Forwards,
                $"{typeof(TAggregate).Name}-{id}",
                StreamPosition.Start,
                cancellationToken: cancellationToken
            );

            var aggregate = new TAggregate();

            await foreach (var @event in readResult)
            {
                var eventData = Deserialize(@event);

                aggregate.When(eventData!);
            }

            return aggregate;
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
    }
}