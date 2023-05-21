using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class DecreaseReservedItemsCommandHandler : ICommandHandler<DecreaseReservedItems>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public DecreaseReservedItemsCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(DecreaseReservedItems command)
        {
            var @event = new ReservedItemsDecreased
            {
                Id = command.Id,
                ItemsReserved = command.ItemsReserved,
                DecreasedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);
        }
    }
}
