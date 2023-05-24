using Common.EventStoreCQRS;
using EventStore.Client;
using OrderApiC.Aggregates;
using OrderApiC.Commands;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Converters;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiC.CommandHandlers
{
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public CancelOrderCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(CancelOrder command)
        {
            //Check if the order already exists
            Order? order = await _eventStore.Find<Order, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} does not exist yet and therfore can not be cancelled");
            }

            var @event = new OrderCanceled
            {
                Id = command.Id,
                Status = new OrderStatusConverter().Convert(command.Status)
            };

            await _eventStore.Append(@event, typeof(Order).Name, _eventSerializer, _cancellationToken);
        }
    }
}
