using SharedModels;
using SharedModels.EventStoreCQRS;
using SharedModels.OrderAPICommon.Events;

namespace OrderApiQ.EventHandlers
{
    public class OrderDeletedEventHandler : IEventHandler<OrderDeleted>
    {
        public Task HandleAsync(OrderDeleted @event, IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {

                var services = scope.ServiceProvider;
                var repository = services.GetService<IRepository<Order>>();

                if (repository.Get(@event.Id) == null)
                    throw new InvalidOperationException("Order not found, cannot remove Order that does not exist.");

                repository.Remove(@event.Id);

                return Task.CompletedTask;
            }
        }
    }
}
