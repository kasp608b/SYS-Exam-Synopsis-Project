using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class ChangeProductPriceCommandHandler : ICommandHandler<ChangeProductPrice>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public ChangeProductPriceCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(ChangeProductPrice command)
        {
            var @event = new ProductPriceChanged
            {
                Id = command.Id,
                Price = command.Price,
                ChangedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);
        }
    }
}
