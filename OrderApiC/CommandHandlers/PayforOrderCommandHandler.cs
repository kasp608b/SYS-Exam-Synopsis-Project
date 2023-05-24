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
    public class PayforOrderCommandHandler : ICommandHandler<PayforOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        private readonly IMessagePublisher _messagePublisher;

        public PayforOrderCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer, IMessagePublisher messagePublisher)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
            _messagePublisher = messagePublisher;
        }

        public async Task HandleAsync(PayforOrder command)
        {
            //Check if the order already exists
            OrderAggregate? order = await _eventStore.Find<OrderAggregate, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} does not exist yet and therfore can not be payed for");
            }

            //Check if the order is already payed for
            if (order.Status == OrderStatus.paid)
            {
                throw new InvalidOperationException($"The order with id:{command.Id} is already payed for");
            }

            var @event = new OrderPayedfor
            {
                Id = command.Id,
                Status = new OrderStatusConverter().Convert(command.Status)
            };

            await _eventStore.Append(@event, typeof(Order).Name, _eventSerializer, _cancellationToken);

            if (await CreditStandingHasChanged(order.OrderId))
            {
                _messagePublisher.PublishCreditStandingChangedMessage(order.CustomerId, true);
            }
        }


        private async Task<bool> CreditStandingHasChanged(Guid customerId)
        {
            HashSet<string> streamNames = new HashSet<string>();
            var prefix = $"{typeof(Order).Name}-";
            var events = _eventStore.ReadStreamAsync(Direction.Forwards, "$all", StreamPosition.Start);

            await foreach (var @event in events)
            {
                if (@event.OriginalStreamId.StartsWith(prefix))
                {
                    streamNames.Add(@event.OriginalStreamId);
                }
            }

            foreach (var streamName in streamNames)
            {
                var order = await _eventStore.Find<OrderAggregate, Guid>(streamName, _eventDeserializer, _cancellationToken);

                if (order.CustomerId == customerId && order.Status == OrderStatus.paid)
                {
                    return true;
                }
            }

            return false;


        }
    }
}
