using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class ChangeProductCategoryCommandHandler : ICommandHandler<ChangeProductCategory>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public ChangeProductCategoryCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }

        public Task HandleAsync(ChangeProductCategory command)
        {
            var @event = new ProductCategoryChanged
            {
                Id = command.Id,
                Category = command.Category,
                ChangedAt = DateTime.UtcNow

            };

            return _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);
        }
    }
}
