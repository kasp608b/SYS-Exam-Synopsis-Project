﻿using Common.EventStoreCQRS;
using EventStore.Client;
using OrderApiC.Aggregates;
using OrderApiC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiC.CommandHandlers
{
    public class DeleteOrderrCommandHandler : ICommandHandler<DeleteOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public DeleteOrderrCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(DeleteOrder command)
        {
            //Check if the order already exists
            OrderAggregate? order = await _eventStore.Find<OrderAggregate, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} does not exist yet and therfore can not be Deleted");
            }

            var @event = new OrderDeleted
            {
                Id = command.Id,
                DeletedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, typeof(OrderAggregate).Name, _eventSerializer, _cancellationToken);
        }
    }
}
