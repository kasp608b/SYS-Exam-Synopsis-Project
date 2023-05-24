using Common.EventStoreCQRS;
using EventStore.Client;
using OrderApiC.Aggregates;
using OrderApiC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Converters;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiC.CommandHandlers
{
    public class ShipOrderCommandHandler : ICommandHandler<ShipOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public ShipOrderCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(ShipOrder command)
        {
            //Check if the order already exists
            Order? order = await _eventStore.Find<Order, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} does not exist yet and therfore can not be shipped");
            }

            var @event = new OrderShipped
            {
                Id = command.Id,
                Status = new OrderStatusConverter().Convert(command.Status)
            };

            await _eventStore.Append(@event, typeof(Order).Name, _eventSerializer, _cancellationToken);
        }
    }
}
