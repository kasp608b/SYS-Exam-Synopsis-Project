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
    public class CancelOrderCommandHandler : ICommandHandler<CancelOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;
        private readonly IMessagePublisher _messagePublisher;



        public CancelOrderCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer, IMessagePublisher messagePublisher)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
            _messagePublisher = messagePublisher;
        }

        public async Task HandleAsync(CancelOrder command)
        {
            //Check if the order already exists
            OrderAggregate? order = await _eventStore.Find<OrderAggregate, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} does not exist yet and therfore can not be cancelled");
            }

            if (order.Status == OrderStatus.cancelled)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} has already been cancelled");
            }

            if (order.Status == OrderStatus.shipped)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} has already been shipped and can not be cancelled");
            }

            if (order.Status == OrderStatus.paid)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} has already been paid and can not be cancelled");
            }





            var @event = new OrderCanceled
            {
                Id = command.Id,
                Status = new OrderStatusConverter().Convert(command.Status)
            };

            await _eventStore.Append(@event, typeof(OrderAggregate).Name, _eventSerializer, _cancellationToken);

            // Publish OrderStatusChangedMessage
            _messagePublisher.PublishOrderStatusChangedMessage(
                order.Id, order.Orderlines.Select(x => new OrderlineConverter().Convert(x)).ToList(), "cancelled");

        }
    }
}
