﻿using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Aggregates;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class RemoveItemsFromStockCommandHandler : ICommandHandler<RemoveItemsFromStock>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public RemoveItemsFromStockCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
            _eventDeserializer = eventDeserializer;
        }

        public async Task HandleAsync(RemoveItemsFromStock command)
        {
            //Check if the product already exists
            Product? product = await _eventStore.Find<Product, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (product == null)
            {
                throw new InvalidOperationException($"The product with id:{command.Id} does not exist yet and therfore can not be updated");
            }
            
            var @event = new ItemsRemovedFromStock
            {
                Id = command.Id,
                ItemsInStock = command.ItemsInStock,
                RemovedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, typeof(Product).Name, _eventSerializer, _cancellationToken);
        }
    }
    
}
