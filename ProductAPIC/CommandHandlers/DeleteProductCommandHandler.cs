using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Aggregates;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class DeleteProductCommandHandler : ICommandHandler<DeleteProduct>
    {

        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public DeleteProductCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
            _eventDeserializer = eventDeserializer;
        }

        public async Task HandleAsync(DeleteProduct command)
        {
            //Check if the product already exists
            ProductAggregate? product = await _eventStore.Find<ProductAggregate, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (product == null)
            {
                throw new InvalidOperationException($"The product with id:{command.Id} does not exist yet and therfore can not be Deleted");
            }
            
            var @event = new ProductDeleted
            {
                Id = command.Id,
                DeletedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);


        }
    }
}
