using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Aggregates;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProduct>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public CreateProductCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }
        public async Task HandleAsync(CreateProduct command)
        {
            //Check if the product already exists
            Product? product = await _eventStore.Find<Product, Guid>(command.Id, _eventDeserializer,_cancellationToken);

            if(product != null)
            {
                throw new InvalidOperationException($"The product with id:{product.Id} is allready created");
            }

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

           await _eventStore.Append(@event, typeof(Product).Name, _eventSerializer, _cancellationToken);
        }
    }
}
