using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProduct>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public CreateProductCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }
        public Task HandleAsync(CreateProduct command)
        {
            var @event = new ProductCreated
            {
                Id = command.Id,
                Name = command.Name,
                Price = command.Price,
                Category = command.Category,
                ItemsInStock = command.ItemsInStock,
                ItemsReserved = command.ItemsReserved,
                CreatedAt = DateTime.UtcNow
            };

            return _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);
        }
    }
}
