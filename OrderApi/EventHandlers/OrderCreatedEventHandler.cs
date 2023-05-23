using OrderApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiQ.EventHandlers
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreated>
    {
        public Task HandleAsync(OrderCreated @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Order>>();

                if (repository.Get(@event.Id) != null)
                    throw new InvalidOperationException("Order already exists, cannot create Order that already exists.");

                Order newOrder = new Order
                {
                    OrderId = @event.Id,
                    CustomerId = @event.CustomerId,
                    Date = @event.Date,
                    Status = new OrderStatusConverter().Convert(@event.Status),
                    Orderlines = @event.Orderlines.Select(x => new OrderlineConverter().Convert(x)).ToList(),
                };

                repository.Add(newOrder);

                return Task.CompletedTask;
            }
        }
    }
}
