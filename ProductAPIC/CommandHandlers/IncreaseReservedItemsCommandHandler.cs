using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Aggregates;
using ProductAPIC.Command;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class IncreaseReservedItemsCommandHandler : ICommandHandler<IncreaseReservedItems>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public IncreaseReservedItemsCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
            _eventDeserializer = eventDeserializer;
        }

        public async Task HandleAsync(IncreaseReservedItems command)
        {
            //Check if the product already exists
            Product? product = await _eventStore.Find<Product, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (product == null)
            {
                throw new InvalidOperationException($"The product with id:{command.Id} does not exist yet and therfore can not be updated");
            }

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
