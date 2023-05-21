using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class ChangeProductNameCommandHandler : ICommandHandler<ChangeProductName>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public ChangeProductNameCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }
        public async Task HandleAsync(ChangeProductName command)
        {
            var @event = new ProductNameChanged
            {
                Id = command.Id,
                Name = command.Name,
                ChangedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);

        }
    }
}
