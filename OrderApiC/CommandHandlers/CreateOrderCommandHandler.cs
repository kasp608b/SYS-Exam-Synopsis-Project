using Common.EventStoreCQRS;
using EventStore.Client;
using OrderApiC.Aggregates;
using OrderApiC.Commands;
using OrderApiC.Models.Converters;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiC.CommandHandlers
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrder>
    {
        private readonly EventStoreClient _eventStore;

        private readonly EventSerializer _eventSerializer;

        private readonly EventDeserializer _eventDeserializer;

        private readonly CancellationToken _cancellationToken;

        public CreateOrderCommandHandler(EventStoreClient eventStore, EventSerializer eventSerializer, EventDeserializer eventDeserializer)
        {
            _eventStore = eventStore;
            _eventSerializer = eventSerializer;
            _eventDeserializer = eventDeserializer;
            _cancellationToken = new CancellationToken();
        }

        public async Task HandleAsync(CreateOrder command)
        {
            //Check if the order already exists
            OrderAggregate? order = await _eventStore.Find<OrderAggregate, Guid>(command.Id, _eventDeserializer, _cancellationToken);

            if (order != null)
            {
                throw new InvalidOperationException($"The order with id:{order.Id} is already created");
            }

            var @event = new OrderCreated
            {
                Id = command.Id,
                CustomerId = command.CustomerId,
                Date = command.Date,
                Status = new OrderStatusConverter().Convert(command.Status),
                Orderlines = command.OrderLines.Select(x => new OrderlineConverter().Convert(x)).ToList(),
                CreatedtAt = DateTime.UtcNow
            };

            await _eventStore.Append(@event, typeof(OrderAggregate).Name, _eventSerializer, _cancellationToken);
        }
    }
}
