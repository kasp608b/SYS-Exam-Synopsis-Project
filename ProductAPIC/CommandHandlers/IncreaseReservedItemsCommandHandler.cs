﻿using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Command;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class IncreaseReservedItemsCommandHandler : ICommandHandler<IncreaseReservedItems>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public IncreaseReservedItemsCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(IncreaseReservedItems command)
        {
            var @event = new ReservedItemsIncreased
            {
                Id = command.Id,
                ItemsReserved = command.ItemsReserved,
                IncreasedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);
            
        }
    }
}
