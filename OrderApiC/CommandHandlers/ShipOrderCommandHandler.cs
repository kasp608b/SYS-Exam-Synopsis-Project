using Common.EventStoreCQRS;
using EventStore.Client;
using OrderApiC.Aggregates;
using OrderApiC.Commands;
using OrderApiC.Infrastructure;
using OrderApiC.Models.Converters;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiC.CommandHandlers
{
    public class ShipOrderCommandHandler : ICommandHandler<ShipOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        private readonly IMessagePublisher _messagePublisher;

        public ShipOrderCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer, IMessagePublisher messagePublisher)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _messagePublisher = messagePublisher;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(ShipOrder command)
        {
            //Check if the order already exists
            OrderAggregate? order = await _eventStore.Find<OrderAggregate, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} does not exist yet and therfore can not be shipped");
            }

            //Check if the order is already shipped
            if (order.Status == OrderStatus.shipped)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} is already shipped");
            }

            var @event = new OrderShipped
            {
                Id = command.Id,
                Status = new OrderStatusConverter().Convert(command.Status)
            };

            await _eventStore.Append(@event, typeof(Order).Name, _eventSerializer, _cancellationToken);

            _messagePublisher.PublishOrderStatusChangedMessage(
                   order.CustomerId, order.Orderlines.Select(x => new OrderlineConverter().Convert(x)).ToList(), "shipped");

        }
    }
}
