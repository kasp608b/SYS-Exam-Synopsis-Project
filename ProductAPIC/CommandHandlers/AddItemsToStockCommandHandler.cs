using Common.EventStoreCQRS;
using EventStore.Client;
using ProductAPIC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.ProductAPICommon.Events;

namespace ProductAPIC.CommandHandlers
{
    public class AddItemsToStockCommandHandler : ICommandHandler<AddItemsToStock>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly CancellationToken _cancellationToken;

        public AddItemsToStockCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(AddItemsToStock command)
        {
            var @event = new ItemsAddedToStock
            {
                Id = command.Id,
                ItemsInStock = command.ItemsInStock,
                AddedAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, "Product", _eventSerializer, _cancellationToken);



        }
    }
    
    }

