using OrderApi.Models;
using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiQ.EventHandlers
{
    public class OrderCanceledEventHandler : IEventHandler<OrderCanceled>
    {
        public Task HandleAsync(OrderCanceled @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Order>>();

                var order = repository.Get(@event.Id);

                if (order == null)
                    throw new InvalidOperationException("Order not found, cannot cancel Order that does not exist.");

                order.Status = new OrderStatusConverter().Convert(@event.Status);

                repository.Edit(order);

                return Task.CompletedTask;
            }
        }
    }
}
