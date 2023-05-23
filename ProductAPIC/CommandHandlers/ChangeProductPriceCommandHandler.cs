using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Aggregates;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class ChangeProductPriceCommandHandler : ICommandHandler<ChangeProductPrice>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;
        
        private readonly CancellationToken _cancellationToken;

        public ChangeProductPriceCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(ChangeProductPrice command)
        {

            //Check if the product already exists
            Product? product = await _eventStore.Find<Product, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (product == null)
            {
                throw new InvalidOperationException($"The product with id:{command.Id} does not exist yet and therfore can not be updated");
            }

            var @event = new ProductPriceChanged
            {
                Id = command.Id,
                Price = command.Price,
                ChangedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, typeof(Product).Name, _eventSerializer, _cancellationToken);
        }
    }
}
